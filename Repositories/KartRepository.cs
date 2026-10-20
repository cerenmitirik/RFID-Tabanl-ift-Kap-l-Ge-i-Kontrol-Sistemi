using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using WinFormsApp1.Models;

namespace WinFormsApp1.Repositories
{
    public class KartRepository : IKartRepository
    {
        private readonly string _connStr;

        public KartRepository(string connectionString)
        {
            _connStr = connectionString;
        }

        private KartBilgisi OkuKart(SqlDataReader reader)
        {
            return new KartBilgisi
            {
                KartUID = reader.GetString(reader.GetOrdinal("KartUID")),
                KartSahibi = reader.GetString(reader.GetOrdinal("KartSahibi")),
                IcerideMi = reader.GetBoolean(reader.GetOrdinal("IcerideMi")),
                AktifMi = reader.GetBoolean(reader.GetOrdinal("AktifMi")),
                ArgeYetkisiVar = reader.GetBoolean(reader.GetOrdinal("ArgeYetkisiVar")),
                ArgeOdasindaMi = reader.GetBoolean(reader.GetOrdinal("ArgeOdasindaMi")),
                IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
            };
        }

        private const string KART_SELECT_SUTUNLARI =
            "KartUID, KartSahibi, IcerideMi, AktifMi, ArgeYetkisiVar, ArgeOdasindaMi, IsAdmin";

        public KartBilgisi KartGetir(string uid)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = $"SELECT {KART_SELECT_SUTUNLARI} FROM TanimliKartlar WHERE KartUID = @uid";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", uid);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return OkuKart(reader);
                    }
                }
            }
            return null;
        }

        public KartBilgisi AdminKartiGetir()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = $"SELECT TOP (1) {KART_SELECT_SUTUNLARI} FROM TanimliKartlar WHERE IsAdmin = 1";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return OkuKart(reader);
                }
            }
            return null;
        }

        // kapi sabit bir enum'dan geldiği için (kullanıcı girdisi değil), sütun adını
        // string birleştirmeyle seçmek güvenlidir; sadece iki değerden biri olabilir.
        public void KartDurumGuncelle(string uid, KapiTipi kapi, bool yeniDurum)
        {
            string kolon = kapi == KapiTipi.BinaKapisi ? "IcerideMi" : "ArgeOdasindaMi";
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = $"UPDATE TanimliKartlar SET {kolon} = @durum WHERE KartUID = @uid";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@durum", yeniDurum);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void KartTanimlaVeyaGuncelle(string uid, string kartSahibi)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string upsertSql = @"
                    IF EXISTS (SELECT 1 FROM TanimliKartlar WHERE KartUID = @uid)
                        UPDATE TanimliKartlar SET KartSahibi = @sahip, AktifMi = 1 WHERE KartUID = @uid
                    ELSE
                        INSERT INTO TanimliKartlar (KartUID, KartSahibi, IcerideMi, AktifMi)
                        VALUES (@uid, @sahip, 0, 1)";
                using (var cmd = new SqlCommand(upsertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@sahip", kartSahibi);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void KartYetkiDurumuGuncelle(string uid, bool aktifMi)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = "UPDATE TanimliKartlar SET AktifMi = @aktif WHERE KartUID = @uid";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@aktif", aktifMi);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ArgeYetkiDurumuGuncelle(string uid, bool argeYetkisiVar)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = "UPDATE TanimliKartlar SET ArgeYetkisiVar = @yetki WHERE KartUID = @uid";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@yetki", argeYetkisiVar);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LogEkle(string uid, string islemTipi, string kapiTipi)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = "INSERT INTO GecisLoglari (KartUID, IslemTipi, KapiTipi, TarihSaat) VALUES (@uid, @islem, @kapi, GETDATE())";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@islem", islemTipi);
                    cmd.Parameters.AddWithValue("@kapi", (object)kapiTipi ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<KartBilgisi> AktifIceridekileriGetir()
        {
            var liste = new List<KartBilgisi>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = $"SELECT {KART_SELECT_SUTUNLARI} FROM TanimliKartlar WHERE IcerideMi = 1 AND AktifMi = 1";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) liste.Add(OkuKart(reader));
                }
            }
            return liste;
        }

        public List<KartBilgisi> TumTanimliKartlariGetir()
        {
            var liste = new List<KartBilgisi>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = $"SELECT {KART_SELECT_SUTUNLARI} FROM TanimliKartlar ORDER BY KartSahibi ASC";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) liste.Add(OkuKart(reader));
                }
            }
            return liste;
        }

        public List<GecisLogu> SonGecisLoglariniGetir(int limit = 30)
        {
            var liste = new List<GecisLogu>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = @"SELECT TOP (@limit) g.Id, g.KartUID,
                                ISNULL(t.KartSahibi, 'TANIMSIZ') AS KartSahibi,
                                g.IslemTipi, g.KapiTipi, g.TarihSaat
                         FROM GecisLoglari g
                         LEFT JOIN TanimliKartlar t ON g.KartUID = t.KartUID
                         ORDER BY g.TarihSaat DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@limit", limit);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int iId = reader.GetOrdinal("Id");
                        int iUid = reader.GetOrdinal("KartUID");
                        int iSahip = reader.GetOrdinal("KartSahibi");
                        int iIslem = reader.GetOrdinal("IslemTipi");
                        int iKapi = reader.GetOrdinal("KapiTipi");
                        int iTarih = reader.GetOrdinal("TarihSaat");

                        while (reader.Read())
                        {
                            liste.Add(new GecisLogu
                            {
                                Id = reader.GetInt32(iId),
                                KartUID = reader.GetString(iUid),
                                KartSahibi = reader.GetString(iSahip),
                                IslemTipi = reader.GetString(iIslem),
                                KapiTipi = reader.IsDBNull(iKapi) ? "" : reader.GetString(iKapi),
                                TarihSaat = reader.GetDateTime(iTarih)
                            });
                        }
                    }
                }
            }
            return liste;
        }

        // --- Filtreleme metodu AYNEN korunuyor, değiştirilmedi ---
        public List<GecisLogu> LoglariFiltrele(LogFiltre f)
        {
            var liste = new List<GecisLogu>();
            var kosullar = new List<string>();

            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    if (f.BaslangicTarih.HasValue)
                    {
                        kosullar.Add("g.TarihSaat >= @basTarih");
                        cmd.Parameters.AddWithValue("@basTarih", f.BaslangicTarih.Value.Date);
                    }
                    if (f.BitisTarih.HasValue)
                    {
                        kosullar.Add("g.TarihSaat < @bitTarih");
                        cmd.Parameters.AddWithValue("@bitTarih", f.BitisTarih.Value.Date.AddDays(1));
                    }
                    if (f.SaatBaslangic.HasValue && f.SaatBitis.HasValue)
                    {
                        if (f.SaatBaslangic.Value <= f.SaatBitis.Value)
                            kosullar.Add("CAST(g.TarihSaat AS time) BETWEEN @saatBas AND @saatBit");
                        else
                            kosullar.Add("(CAST(g.TarihSaat AS time) >= @saatBas OR CAST(g.TarihSaat AS time) <= @saatBit)");
                        cmd.Parameters.AddWithValue("@saatBas", f.SaatBaslangic.Value);
                        cmd.Parameters.AddWithValue("@saatBit", f.SaatBitis.Value);
                    }
                    if (!string.IsNullOrWhiteSpace(f.KisiAdi))
                    {
                        kosullar.Add("ISNULL(t.KartSahibi, 'TANIMSIZ') LIKE @kisi");
                        cmd.Parameters.AddWithValue("@kisi", "%" + f.KisiAdi.Trim() + "%");
                    }

                    string where = kosullar.Count > 0 ? " WHERE " + string.Join(" AND ", kosullar) : "";

                    cmd.CommandText = @"SELECT g.Id, g.KartUID,
                                               ISNULL(t.KartSahibi, 'TANIMSIZ') AS KartSahibi,
                                               g.IslemTipi, g.KapiTipi, g.TarihSaat
                                        FROM GecisLoglari g
                                        LEFT JOIN TanimliKartlar t ON g.KartUID = t.KartUID"
                                      + where + " ORDER BY g.TarihSaat DESC";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new GecisLogu
                            {
                                Id = reader.GetInt32(0),
                                KartUID = reader.GetString(1),
                                KartSahibi = reader.GetString(2),
                                IslemTipi = reader.GetString(3),
                                KapiTipi = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                TarihSaat = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return liste;
        }
    }
}