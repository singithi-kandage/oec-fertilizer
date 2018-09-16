using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SKOEC.Models
{
    public partial class OECContext : DbContext
    {
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Crop> Crop { get; set; }
        public virtual DbSet<Farm> Farm { get; set; }
        public virtual DbSet<Fertilizer> Fertilizer { get; set; }
        public virtual DbSet<Plot> Plot { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Treatment> Treatment { get; set; }
        public virtual DbSet<TreatmentFertilizer> TreatmentFertilizer { get; set; }
        public virtual DbSet<Variety> Variety { get; set; }

        //Context constructor expecting the connection string as an option
        public OECContext(DbContextOptions<OECContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CountryCode);

                entity.ToTable("country");

                entity.Property(e => e.CountryCode)
                    .HasColumnName("countryCode")
                    .HasColumnType("nchar(2)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.PhonePattern)
                    .HasColumnName("phonePattern")
                    .HasMaxLength(50);

                entity.Property(e => e.PostalPattern)
                    .HasColumnName("postalPattern")
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<Crop>(entity =>
            {
                entity.ToTable("crop");

                entity.Property(e => e.CropId).HasColumnName("cropId");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(30);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Farm>(entity =>
            {
                entity.ToTable("farm");

                entity.HasIndex(e => e.ProvinceCode)
                    .HasName("province code");

                entity.Property(e => e.FarmId).HasColumnName("farmId");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CellPhone)
                    .HasColumnName("cellPhone")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.County)
                    .HasColumnName("county")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateJoined)
                    .HasColumnName("dateJoined")
                    .HasColumnType("datetime");

                entity.Property(e => e.Directions)
                    .HasColumnName("directions")
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.HomePhone)
                    .HasColumnName("homePhone")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastContactDate)
                    .HasColumnName("lastContactDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postalCode")
                    .HasColumnType("nchar(7)");

                entity.Property(e => e.ProvinceCode)
                    .IsRequired()
                    .HasColumnName("provinceCode")
                    .HasColumnType("nchar(2)");

                entity.Property(e => e.Town)
                    .HasColumnName("town")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.Farm)
                    .HasForeignKey(d => d.ProvinceCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_farm_province");
            });

            modelBuilder.Entity<Fertilizer>(entity =>
            {
                entity.HasKey(e => e.FertilizerName)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("fertilizer");

                entity.Property(e => e.FertilizerName)
                    .HasColumnName("fertilizerName")
                    .HasMaxLength(255)
                    .ValueGeneratedNever();

                entity.Property(e => e.Liquid).HasColumnName("liquid");

                entity.Property(e => e.Oecproduct).HasColumnName("OECProduct");
            });

            modelBuilder.Entity<Plot>(entity =>
            {
                entity.ToTable("plot");

                entity.HasIndex(e => e.FarmId)
                    .HasName("locationID");

                entity.HasIndex(e => e.VarietyId)
                    .HasName("cropID");

                entity.Property(e => e.PlotId).HasColumnName("plotId");

                entity.Property(e => e.BicarbP).HasColumnName("bicarbP");

                entity.Property(e => e.Calcium).HasColumnName("calcium");

                entity.Property(e => e.Cec).HasColumnName("CEC");

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasColumnType("ntext");

                entity.Property(e => e.DateHarvested)
                    .HasColumnName("dateHarvested")
                    .HasColumnType("datetime");

                entity.Property(e => e.DatePlanted)
                    .HasColumnName("datePlanted")
                    .HasColumnType("datetime");

                entity.Property(e => e.FarmId).HasColumnName("farmId");

                entity.Property(e => e.Magnesium).HasColumnName("magnesium");

                entity.Property(e => e.OrganicMatter).HasColumnName("organicMatter");

                entity.Property(e => e.PHbuffer).HasColumnName("pHBuffer");

                entity.Property(e => e.PHsoil).HasColumnName("pHSoil");

                entity.Property(e => e.PatternRepeats).HasColumnName("patternRepeats");

                entity.Property(e => e.PercentBaseSaturationCa).HasColumnName("percentBaseSaturationCa");

                entity.Property(e => e.PercentBaseSaturationH).HasColumnName("percentBaseSaturationH");

                entity.Property(e => e.PercentBaseSaturationK).HasColumnName("percentBaseSaturationK");

                entity.Property(e => e.PercentBaseSaturationMg).HasColumnName("percentBaseSaturationMg");

                entity.Property(e => e.PlantingRate).HasColumnName("plantingRate");

                entity.Property(e => e.PlantingRateByPounds).HasColumnName("plantingRateByPounds");

                entity.Property(e => e.Potassium).HasColumnName("potassium");

                entity.Property(e => e.RowWidth).HasColumnName("rowWidth");

                entity.Property(e => e.VarietyId).HasColumnName("varietyId");

                entity.HasOne(d => d.Farm)
                    .WithMany(p => p.Plot)
                    .HasForeignKey(d => d.FarmId)
                    .HasConstraintName("plots_FK00");

                entity.HasOne(d => d.Variety)
                    .WithMany(p => p.Plot)
                    .HasForeignKey(d => d.VarietyId)
                    .HasConstraintName("plots_FK01");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.ProvinceCode);

                entity.ToTable("province");

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasColumnType("nchar(2)")
                    .ValueGeneratedNever();

                entity.Property(e => e.CountryCode)
                    .HasColumnName("countryCode")
                    .HasColumnType("nchar(2)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("nchar(30)");

                entity.HasOne(d => d.CountryCodeNavigation)
                    .WithMany(p => p.Province)
                    .HasForeignKey(d => d.CountryCode)
                    .HasConstraintName("FK_province_country");
            });

            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.ToTable("treatment");

                entity.Property(e => e.TreatmentId).HasColumnName("treatmentId");

                entity.Property(e => e.Moisture).HasColumnName("moisture");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.Property(e => e.PlotId).HasColumnName("plotId");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.Property(e => e.Yield).HasColumnName("yield");

                entity.HasOne(d => d.Plot)
                    .WithMany(p => p.Treatment)
                    .HasForeignKey(d => d.PlotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_treatment_plot");
            });

            modelBuilder.Entity<TreatmentFertilizer>(entity =>
            {
                entity.ToTable("treatmentFertilizer");

                entity.Property(e => e.TreatmentFertilizerId).HasColumnName("treatmentFertilizerId");

                entity.Property(e => e.FertilizerName)
                    .HasColumnName("fertilizerName")
                    .HasMaxLength(255);

                entity.Property(e => e.RateMetric)
                    .HasColumnName("rateMetric")
                    .HasMaxLength(255);

                entity.Property(e => e.RatePerAcre).HasColumnName("ratePerAcre");

                entity.Property(e => e.TreatmentId).HasColumnName("treatmentId");

                entity.HasOne(d => d.FertilizerNameNavigation)
                    .WithMany(p => p.TreatmentFertilizer)
                    .HasForeignKey(d => d.FertilizerName)
                    .HasConstraintName("FK_treatmentFertilizer_fertilizer");

                entity.HasOne(d => d.Treatment)
                    .WithMany(p => p.TreatmentFertilizer)
                    .HasForeignKey(d => d.TreatmentId)
                    .HasConstraintName("FK_treatmentFertilizer_treatment");
            });

            modelBuilder.Entity<Variety>(entity =>
            {
                entity.ToTable("variety");

                entity.HasIndex(e => e.CropId)
                    .HasName("cropID");

                entity.Property(e => e.VarietyId).HasColumnName("varietyId");

                entity.Property(e => e.CropId).HasColumnName("cropId");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Crop)
                    .WithMany(p => p.Variety)
                    .HasForeignKey(d => d.CropId)
                    .HasConstraintName("cropVariety_FK00");
            });
        }
    }
}
