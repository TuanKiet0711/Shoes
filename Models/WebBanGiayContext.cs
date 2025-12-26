using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebBanGiay.Models;

public partial class WebBanGiayContext : DbContext
{
    public WebBanGiayContext()
    {
    }

    public WebBanGiayContext(DbContextOptions<WebBanGiayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BienTheSanPham> BienTheSanPham { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHang { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHang { get; set; }

    public virtual DbSet<DanhGia> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMuc { get; set; }

    public virtual DbSet<DonHang> DonHang { get; set; }

    public virtual DbSet<GiamGia> GiamGia { get; set; }

    public virtual DbSet<GioHang> GioHang { get; set; }

    public virtual DbSet<HinhAnhSanPham> HinhAnhSanPham { get; set; }

    public virtual DbSet<PhanCongTaiXe> PhanCongTaiXe { get; set; }

    public virtual DbSet<SanPham> SanPham { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoan { get; set; }

    public virtual DbSet<TaiXe> TaiXe { get; set; }

    public virtual DbSet<ThanhToan> ThanhToan { get; set; }

    public virtual DbSet<ThongTinGiaoHang> ThongTinGiaoHang { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieu { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=WebBanGiay;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BienTheSanPham>(entity =>
        {
            entity.HasKey(e => e.MaBienThe).HasName("PK__BienTheS__3987CEF502E2F2C6");

            entity.Property(e => e.MaBienThe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MauSac).HasMaxLength(50);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.BienTheSanPham)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__BienTheSa__MaSan__44FF419A");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDonHang).HasName("PK__ChiTietD__4B0B45DD031D195A");

            entity.ToTable(tb => tb.HasTrigger("trg_TruSoLuongKho"));

            entity.Property(e => e.MaChiTietDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaBienThe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.ChiTietDonHang)
                .HasForeignKey(d => d.MaBienThe)
                .HasConstraintName("FK__ChiTietDo__MaBie__6754599E");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHang)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ChiTietDo__MaDon__66603565");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.MaChiTietGioHang).HasName("PK__ChiTietG__BBF47498FE535A56");

            entity.Property(e => e.MaChiTietGioHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaBienThe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaGioHang)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.ChiTietGioHang)
                .HasForeignKey(d => d.MaBienThe)
                .HasConstraintName("FK__ChiTietGi__MaBie__5812160E");

            entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.ChiTietGioHang)
                .HasForeignKey(d => d.MaGioHang)
                .HasConstraintName("FK__ChiTietGi__MaGio__571DF1D5");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGia__AA9515BF8F2344B6");

            entity.Property(e => e.MaDanhGia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__DanhGia__MaSanPh__4CA06362");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK__DanhGia__MaTaiKh__4D94879B");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PK__DanhMuc__B37508874449E3AA");

            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD90A237E1");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_TinhTienGiamGia");
                    tb.HasTrigger("trg_TinhTongCong");
                });

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LyDoHuy).HasMaxLength(255);
            entity.Property(e => e.MaGiamGia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayDatHang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhiVanChuyen).HasDefaultValue(30000);
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(20);
            entity.Property(e => e.TrangThaiDonHang).HasMaxLength(30);
            entity.Property(e => e.TrangThaiThanhToan).HasMaxLength(20);

            entity.HasOne(d => d.MaGiamGiaNavigation).WithMany(p => p.DonHang)
                .HasForeignKey(d => d.MaGiamGia)
                .HasConstraintName("FK__DonHang__MaGiamG__6383C8BA");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.DonHang)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK__DonHang__MaTaiKh__628FA481");
        });

        modelBuilder.Entity<GiamGia>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK__GiamGia__EF9458E4E8F80AF6");

            entity.Property(e => e.MaGiamGia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LoaiGiamGia).HasMaxLength(20);
            entity.Property(e => e.TenMa).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(20);
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GioHang__F5001DA33F7FBAD8");

            entity.Property(e => e.MaGioHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.GioHang)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK__GioHang__MaTaiKh__5441852A");
        });

        modelBuilder.Entity<HinhAnhSanPham>(entity =>
        {
            entity.HasKey(e => e.MaHinhAnh).HasName("PK__HinhAnhS__A9C37A9B72670278");

            entity.Property(e => e.MaHinhAnh)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DuongDanAnh).HasMaxLength(255);
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.HinhAnhSanPham)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__HinhAnhSa__MaSan__47DBAE45");
        });

        modelBuilder.Entity<PhanCongTaiXe>(entity =>
        {
            entity.HasKey(e => e.MaPhanCong).HasName("PK__PhanCong__C279D916A7ACB285");

            entity.Property(e => e.MaPhanCong)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LyDoThatBai).HasMaxLength(255);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaTaiXe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhiGiaoHang).HasDefaultValue(20000);
            entity.Property(e => e.ThoiGianHoanThanh).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianLayHang).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianNhanHang).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianPhanCong).HasColumnType("datetime");
            entity.Property(e => e.TrangThaiGiao).HasMaxLength(30);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.PhanCongTaiXe)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__PhanCongT__MaDon__74AE54BC");

            entity.HasOne(d => d.MaTaiXeNavigation).WithMany(p => p.PhanCongTaiXe)
                .HasForeignKey(d => d.MaTaiXe)
                .HasConstraintName("FK__PhanCongT__MaTai__75A278F5");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D61DA5DF4");

            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaThuongHieu)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenSanPham).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPham)
                .HasForeignKey(d => d.MaDanhMuc)
                .HasConstraintName("FK__SanPham__MaDanhM__412EB0B6");

            entity.HasOne(d => d.MaThuongHieuNavigation).WithMany(p => p.SanPham)
                .HasForeignKey(d => d.MaThuongHieu)
                .HasConstraintName("FK__SanPham__MaThuon__4222D4EF");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C6529AFA2E203");

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D10534BF5058C9").IsUnique();

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HanDatLaiMatKhau).HasColumnType("datetime");
            entity.Property(e => e.HanTokenEmail).HasColumnType("datetime");
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
            entity.Property(e => e.TokenDatLaiMatKhau).HasMaxLength(255);
            entity.Property(e => e.TokenXacThucEmail).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(20);
            entity.Property(e => e.VaiTro).HasMaxLength(20);

            entity.HasMany(d => d.MaSanPham).WithMany(p => p.MaTaiKhoan)
                .UsingEntity<Dictionary<string, object>>(
                    "YeuThich",
                    r => r.HasOne<SanPham>().WithMany()
                        .HasForeignKey("MaSanPham")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__YeuThich__MaSanP__5165187F"),
                    l => l.HasOne<TaiKhoan>().WithMany()
                        .HasForeignKey("MaTaiKhoan")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__YeuThich__MaTaiK__5070F446"),
                    j =>
                    {
                        j.HasKey("MaTaiKhoan", "MaSanPham").HasName("PK__YeuThich__62D0116B5DD82093");
                        j.IndexerProperty<string>("MaTaiKhoan")
                            .HasMaxLength(50)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("MaSanPham")
                            .HasMaxLength(50)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<TaiXe>(entity =>
        {
            entity.HasKey(e => e.MaTaiXe).HasName("PK__TaiXe__FA9D79BE67028CCA");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__TaiXe__AD7C6528516D8619").IsUnique();

            entity.Property(e => e.MaTaiXe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.TaiXe)
                .HasForeignKey<TaiXe>(d => d.MaTaiKhoan)
                .HasConstraintName("FK__TaiXe__MaTaiKhoa__70DDC3D8");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B258449225AC49");

            entity.Property(e => e.MaThanhToan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhuongThuc).HasMaxLength(20);
            entity.Property(e => e.ThoiGianThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThanhToan)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ThanhToan__MaDon__6D0D32F4");
        });

        modelBuilder.Entity<ThongTinGiaoHang>(entity =>
        {
            entity.HasKey(e => e.MaThongTinGiaoHang).HasName("PK__ThongTin__62F5C763F4E4B51D");

            entity.ToTable(tb => tb.HasTrigger("trg_TinhNgayNhanHangDuKien"));

            entity.Property(e => e.MaThongTinGiaoHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DiaChiNguoiNhan).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.HoTenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhuongXa).HasMaxLength(100);
            entity.Property(e => e.QuanHuyen).HasMaxLength(100);
            entity.Property(e => e.SdtnguoiNhan)
                .HasMaxLength(20)
                .HasColumnName("SDTNguoiNhan");
            entity.Property(e => e.TinhThanh).HasMaxLength(100);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThongTinGiaoHang)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ThongTinG__MaDon__6A30C649");
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.MaThuongHieu).HasName("PK__ThuongHi__A3733E2CD9277BB2");

            entity.Property(e => e.MaThuongHieu)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenThuongHieu).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
