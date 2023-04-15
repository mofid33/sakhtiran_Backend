using MarketPlace.API.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;


namespace MarketPlace.API.Data.Models
{
    public partial class MarketPlaceDbContext : DbContext
    {
        public MarketPlaceDbContext()
        {
        }

        public MarketPlaceDbContext(DbContextOptions<MarketPlaceDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        public virtual DbSet<Counter> Counter { get; set; }
        public virtual DbSet<Hash> Hash { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobParameter> JobParameter { get; set; }
        public virtual DbSet<JobQueue> JobQueue { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<Schema> Schema { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<TAgreement> TAgreement { get; set; }
        public virtual DbSet<TBrand> TBrand { get; set; }
        public virtual DbSet<TCallRequest> TCallRequest { get; set; }
        public virtual DbSet<TCallRequestStatus> TCallRequestStatus { get; set; }
        public virtual DbSet<TCategory> TCategory { get; set; }
        public virtual DbSet<TCategoryBrand> TCategoryBrand { get; set; }
        public virtual DbSet<TCategoryGuarantee> TCategoryGuarantee { get; set; }
        public virtual DbSet<TCategorySpecification> TCategorySpecification { get; set; }
        public virtual DbSet<TCategorySpecificationGroup> TCategorySpecificationGroup { get; set; }
        public virtual DbSet<TCity> TCity { get; set; }
        public virtual DbSet<TCodeRepository> TCodeRepository { get; set; }
        public virtual DbSet<TCountry> TCountry { get; set; }
        public virtual DbSet<TCurrency> TCurrency { get; set; }
        public virtual DbSet<TCustomer> TCustomer { get; set; }
        public virtual DbSet<TCustomerAddress> TCustomerAddress { get; set; }
        public virtual DbSet<TCustomerBankCard> TCustomerBankCard { get; set; }
        public virtual DbSet<TDiscountCategory> TDiscountCategory { get; set; }
        public virtual DbSet<TDiscountCouponCode> TDiscountCouponCode { get; set; }
        public virtual DbSet<TDiscountCouponCodeType> TDiscountCouponCodeType { get; set; }
        public virtual DbSet<TDiscountCustomers> TDiscountCustomers { get; set; }
        public virtual DbSet<TDiscountFreeGoods> TDiscountFreeGoods { get; set; }
        public virtual DbSet<TDiscountGoods> TDiscountGoods { get; set; }
        public virtual DbSet<TDiscountPlan> TDiscountPlan { get; set; }
        public virtual DbSet<TDiscountPlanType> TDiscountPlanType { get; set; }
        public virtual DbSet<TDiscountRangeType> TDiscountRangeType { get; set; }
        public virtual DbSet<TDiscountShops> TDiscountShops { get; set; }
        public virtual DbSet<TDiscountType> TDiscountType { get; set; }
        public virtual DbSet<TDocumentGroup> TDocumentGroup { get; set; }
        public virtual DbSet<TDocumentType> TDocumentType { get; set; }
        public virtual DbSet<TForceUpdate> TForceUpdate { get; set; }
        public virtual DbSet<TGoods> TGoods { get; set; }
        public virtual DbSet<TGoodsComment> TGoodsComment { get; set; }
        public virtual DbSet<TGoodsCommentPoints> TGoodsCommentPoints { get; set; }
        public virtual DbSet<TGoodsDocument> TGoodsDocument { get; set; }
        public virtual DbSet<TGoodsLike> TGoodsLike { get; set; }
        public virtual DbSet<TGoodsProvider> TGoodsProvider { get; set; }
        public virtual DbSet<TGoodsQueAns> TGoodsQueAns { get; set; }
        public virtual DbSet<TGoodsSpecification> TGoodsSpecification { get; set; }
        public virtual DbSet<TGoodsSpecificationOptions> TGoodsSpecificationOptions { get; set; }
        public virtual DbSet<TGoodsSurveyAnswers> TGoodsSurveyAnswers { get; set; }
        public virtual DbSet<TGoodsSurveyQuestions> TGoodsSurveyQuestions { get; set; }
        public virtual DbSet<TGoodsVariety> TGoodsVariety { get; set; }
        public virtual DbSet<TGoodsView> TGoodsView { get; set; }
        public virtual DbSet<TGuarantee> TGuarantee { get; set; }
        public virtual DbSet<THelpArticle> THelpArticle { get; set; }
        public virtual DbSet<THelpTopic> THelpTopic { get; set; }
        public virtual DbSet<TLanguage> TLanguage { get; set; }
        public virtual DbSet<TMeasurementUnit> TMeasurementUnit { get; set; }
        public virtual DbSet<TMenuItem> TMenuItem { get; set; }
        public virtual DbSet<TMessage> TMessage { get; set; }
        public virtual DbSet<TMessageAttachment> TMessageAttachment { get; set; }
        public virtual DbSet<TMessageRecipient> TMessageRecipient { get; set; }
        public virtual DbSet<TNotificationSetting> TNotificationSetting { get; set; }
        public virtual DbSet<TOrder> TOrder { get; set; }
        public virtual DbSet<TOrderCanceling> TOrderCanceling { get; set; }
        public virtual DbSet<TOrderCancelingReason> TOrderCancelingReason { get; set; }
        public virtual DbSet<TOrderItem> TOrderItem { get; set; }
        public virtual DbSet<TOrderLog> TOrderLog { get; set; }
        public virtual DbSet<TOrderReturning> TOrderReturning { get; set; }
        public virtual DbSet<TOrderReturningLog> TOrderReturningLog { get; set; }
        public virtual DbSet<TOrderStatus> TOrderStatus { get; set; }
        public virtual DbSet<TPaymentMethod> TPaymentMethod { get; set; }
        public virtual DbSet<TPaymentTransaction> TPaymentTransaction { get; set; }
        public virtual DbSet<TPerson> TPerson { get; set; }
        public virtual DbSet<TPopupItem> TPopupItem { get; set; }
        public virtual DbSet<TProvince> TProvince { get; set; }
        public virtual DbSet<TRecommendation> TRecommendation { get; set; }
        public virtual DbSet<TRecommendationCollectionType> TRecommendationCollectionType { get; set; }
        public virtual DbSet<TRecommendationItemType> TRecommendationItemType { get; set; }
        public virtual DbSet<TReturningAction> TReturningAction { get; set; }
        public virtual DbSet<TReturningReason> TReturningReason { get; set; }
        public virtual DbSet<TReturningStatus> TReturningStatus { get; set; }
        public virtual DbSet<TSetting> TSetting { get; set; }
        public virtual DbSet<TSettingPayment> TSettingPayment { get; set; }
        public virtual DbSet<TShippingMethod> TShippingMethod { get; set; }
        public virtual DbSet<TShippingMethodAreaCode> TShippingMethodAreaCode { get; set; }
        public virtual DbSet<TShippingOnCity> TShippingOnCity { get; set; }
        public virtual DbSet<TShippingOnCountry> TShippingOnCountry { get; set; }
        public virtual DbSet<TShop> TShop { get; set; }
        public virtual DbSet<TShopActivityCity> TShopActivityCity { get; set; }
        public virtual DbSet<TShopActivityCountry> TShopActivityCountry { get; set; }
        public virtual DbSet<TShopCategory> TShopCategory { get; set; }
        public virtual DbSet<TShopComment> TShopComment { get; set; }
        public virtual DbSet<TShopCommentLike> TShopCommentLike { get; set; }
        public virtual DbSet<TShopFiles> TShopFiles { get; set; }
        public virtual DbSet<TShopPlan> TShopPlan { get; set; }
        public virtual DbSet<TShopPlanExclusive> TShopPlanExclusive { get; set; }
        public virtual DbSet<TShopSlider> TShopSlider { get; set; }
        public virtual DbSet<TShopStatus> TShopStatus { get; set; }
        public virtual DbSet<TShopSurveyAnswers> TShopSurveyAnswers { get; set; }
        public virtual DbSet<TShopSurveyQuestions> TShopSurveyQuestions { get; set; }
        public virtual DbSet<TShopWithdrawalRequest> TShopWithdrawalRequest { get; set; }
        public virtual DbSet<TSmstext> TSmstext { get; set; }
        public virtual DbSet<TSpecification> TSpecification { get; set; }
        public virtual DbSet<TSpecificationGroup> TSpecificationGroup { get; set; }
        public virtual DbSet<TSpecificationOptions> TSpecificationOptions { get; set; }
        public virtual DbSet<TStockOperation> TStockOperation { get; set; }
        public virtual DbSet<TStockOperationType> TStockOperationType { get; set; }
        public virtual DbSet<TTransactionStatus> TTransactionStatus { get; set; }
        public virtual DbSet<TTransactionType> TTransactionType { get; set; }
        public virtual DbSet<TUser> TUser { get; set; }
        public virtual DbSet<TUserAccessControl> TUserAccessControl { get; set; }
        public virtual DbSet<TUserGroup> TUserGroup { get; set; }
        public virtual DbSet<TUserTransaction> TUserTransaction { get; set; }
        public virtual DbSet<TVariationParameter> TVariationParameter { get; set; }
        public virtual DbSet<TVariationParameterValues> TVariationParameterValues { get; set; }
        public virtual DbSet<TVariationPerCategory> TVariationPerCategory { get; set; }
        public virtual DbSet<TVerification> TVerification { get; set; }
        public virtual DbSet<TWorkingShift> TWorkingShift { get; set; }
        public virtual DbSet<WebCollectionType> WebCollectionType { get; set; }
        public virtual DbSet<WebIndexModuleList> WebIndexModuleList { get; set; }
        public virtual DbSet<WebModule> WebModule { get; set; }
        public virtual DbSet<WebModuleCollections> WebModuleCollections { get; set; }
        public virtual DbSet<WebSlider> WebSlider { get; set; }
        public virtual DbSet<WebSliderItems> WebSliderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("data source=185.192.112.16;initial catalog=podinis_com_shop;user id=adminShop;password=Afcv34^9;MultipleActiveResultSets=True;App=EntityFramework");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            
            modelBuilder.HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
            .HasTranslation(e => SqlFunctionExpression.Create(
                "JSON_VALUE", e, typeof(string), null));

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key)
                    .HasName("CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameter)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<TAgreement>(entity =>
            {
                entity.HasKey(e => e.TypeId);

                entity.ToTable("tAgreement", "dbo");

                entity.Property(e => e.TypeId).ValueGeneratedNever();

                entity.Property(e => e.TypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasComment("test");
            });

            modelBuilder.Entity<TBrand>(entity =>
            {
                entity.HasKey(e => e.BrandId);

                entity.ToTable("tBrand", "dbo");

                entity.Property(e => e.BrandLogoImage)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.BrandTitle)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.IsAccepted).HasColumnName("isAccepted");
            });

            modelBuilder.Entity<TCallRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("tCallRequest", "dbo");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkGoodsProviderId).HasColumnName("FK_GoodsProviderId");

                entity.Property(e => e.FkStatusId).HasColumnName("FK_StatusId");

                entity.Property(e => e.RequestDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TCallRequest)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCallRequest_tCustomer");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TCallRequest)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCallRequest_tGoods");

                entity.HasOne(d => d.FkGoodsProvider)
                    .WithMany(p => p.TCallRequest)
                    .HasForeignKey(d => d.FkGoodsProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCallRequest_tGoodsProvider");

                entity.HasOne(d => d.FkStatus)
                    .WithMany(p => p.TCallRequest)
                    .HasForeignKey(d => d.FkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCallRequest_tCallRequestStatus");
            });

            modelBuilder.Entity<TCallRequestStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("tCallRequestStatus", "dbo");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<TCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK_tGoodsCategory");

                entity.ToTable("tCategory", "dbo");

                entity.Property(e => e.CategoryPath)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryTitle)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.FkParentId).HasColumnName("FK_ParentId");

                entity.Property(e => e.IconUrl).HasMaxLength(4000);

                entity.Property(e => e.ImageUrl).HasMaxLength(4000);

                entity.Property(e => e.MetaDescription).HasColumnName("Meta_Description");

                entity.Property(e => e.MetaKeywords).HasColumnName("Meta_Keywords");

                entity.Property(e => e.MetaTitle).HasColumnName("Meta_Title");

                entity.HasOne(d => d.FkParent)
                    .WithMany(p => p.InverseFkParent)
                    .HasForeignKey(d => d.FkParentId)
                    .HasConstraintName("FK_tCategory_tCategory");
            });

            modelBuilder.Entity<TCategoryBrand>(entity =>
            {
                entity.HasKey(e => e.BrandCategoryId)
                    .HasName("PK_tBrandCategory");

                entity.ToTable("tCategoryBrand", "dbo");

                entity.Property(e => e.FkBrandId).HasColumnName("FK_BrandId");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.HasOne(d => d.FkBrand)
                    .WithMany(p => p.TCategoryBrand)
                    .HasForeignKey(d => d.FkBrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tBrandCategory_tBrand");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TCategoryBrand)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tBrandCategory_tCategory");
            });

            modelBuilder.Entity<TCategoryGuarantee>(entity =>
            {
                entity.HasKey(e => e.CategoryGuaranteeId);

                entity.ToTable("tCategoryGuarantee", "dbo");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkGuaranteeId).HasColumnName("FK_GuaranteeId");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TCategoryGuarantee)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCategoryGuarantee_tCategory");

                entity.HasOne(d => d.FkGuarantee)
                    .WithMany(p => p.TCategoryGuarantee)
                    .HasForeignKey(d => d.FkGuaranteeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCategoryGuarantee_tGuaranteeType");
            });

            modelBuilder.Entity<TCategorySpecification>(entity =>
            {
                entity.HasKey(e => e.Gcsid)
                    .HasName("PK_tGoodsCategorySpecification");

                entity.ToTable("tCategorySpecification", "dbo");

                entity.Property(e => e.Gcsid).HasColumnName("GCSId");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkSpecId).HasColumnName("FK_SpecId");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TCategorySpecification)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsCategorySpecification_tGoodsCategory");

                entity.HasOne(d => d.FkSpec)
                    .WithMany(p => p.TCategorySpecification)
                    .HasForeignKey(d => d.FkSpecId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsCategorySpecification_tSpecification");
            });

            modelBuilder.Entity<TCategorySpecificationGroup>(entity =>
            {
                entity.HasKey(e => e.CatSpecGroupId);

                entity.ToTable("tCategorySpecificationGroup", "dbo");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkSpecGroupId).HasColumnName("FK_SpecGroupId");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TCategorySpecificationGroup)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCategorySpecificationGroup_tCategory");

                entity.HasOne(d => d.FkSpecGroup)
                    .WithMany(p => p.TCategorySpecificationGroup)
                    .HasForeignKey(d => d.FkSpecGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCategorySpecificationGroup_tSpecificationGroup");
            });

            modelBuilder.Entity<TCity>(entity =>
            {
                entity.HasKey(e => e.CityId)
                    .HasName("PK_dbo.tCity");

                entity.ToTable("tCity", "dbo");

                entity.Property(e => e.CName)
                    .HasColumnName("c_name")
                    .HasMaxLength(150);

                entity.Property(e => e.CityTitle)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.FkProvinceId).HasColumnName("FK_ProvinceId");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TCity)
                    .HasForeignKey(d => d.FkCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCity_tCountry");

                entity.HasOne(d => d.FkProvince)
                    .WithMany(p => p.TCity)
                    .HasForeignKey(d => d.FkProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCity_tProvince");
            });

            modelBuilder.Entity<TCodeRepository>(entity =>
            {
                entity.HasKey(e => e.DiscountCode);

                entity.ToTable("tCodeRepository", "dbo");

                entity.Property(e => e.DiscountCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TCountry>(entity =>
            {
                entity.HasKey(e => e.CountryId)
                    .HasName("PK_dbo.tProvince");

                entity.ToTable("tCountry", "dbo");

                entity.Property(e => e.CountryId).ValueGeneratedNever();

                entity.Property(e => e.CountryTitle)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.FlagUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Iso2)
                    .HasColumnName("ISO2")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Iso3)
                    .HasColumnName("ISO3")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneCode)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<TCurrency>(entity =>
            {
                entity.HasKey(e => e.CurrencyId);

                entity.ToTable("tCurrency", "dbo");

                entity.Property(e => e.CurrencyId).ValueGeneratedNever();

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyTitle)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TCustomer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.ToTable("tCustomer", "dbo");

                entity.Property(e => e.BankCard)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.BirthDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Credit).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Family).HasMaxLength(50);

                entity.Property(e => e.FkCityId).HasColumnName("FK_CityId");

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.FkProvinceId).HasColumnName("FK_ProvinceId");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.NationalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RegisteryDate).HasColumnType("date");

                entity.Property(e => e.TelNumber)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TempDiscountCode)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.TCustomer)
                    .HasForeignKey(d => d.FkCityId)
                    .HasConstraintName("FK_tCustomer_tCity");

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TCustomer)
                    .HasForeignKey(d => d.FkCountryId)
                    .HasConstraintName("FK_tCustomer_tCountry");

                entity.HasOne(d => d.FkProvince)
                    .WithMany(p => p.TCustomer)
                    .HasForeignKey(d => d.FkProvinceId)
                    .HasConstraintName("FK_tCustomer_tProvince");
            });

            modelBuilder.Entity<TCustomerAddress>(entity =>
            {
                entity.HasKey(e => e.AddressId);

                entity.ToTable("tCustomerAddress", "dbo");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FkCityId).HasColumnName("FK_CityId");

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkProvinceId).HasColumnName("FK_ProvinceId");

                entity.Property(e => e.LocationX).HasColumnName("Location_X");

                entity.Property(e => e.LocationY).HasColumnName("Location_Y");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransfereeFamily).HasMaxLength(40);

                entity.Property(e => e.TransfereeMobile)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TransfereeName).HasMaxLength(30);

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.TCustomerAddress)
                    .HasForeignKey(d => d.FkCityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCustomerAddress_tCity");

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TCustomerAddress)
                    .HasForeignKey(d => d.FkCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCustomerAddress_tProvince");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TCustomerAddress)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCustomerAddress_tCustomer");

                entity.HasOne(d => d.FkProvince)
                    .WithMany(p => p.TCustomerAddress)
                    .HasForeignKey(d => d.FkProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCustomerAddress_tProvince1");
            });

            modelBuilder.Entity<TCustomerBankCard>(entity =>
            {
                entity.HasKey(e => e.BankCardId);

                entity.ToTable("tCustomerBankCard", "dbo");

                entity.Property(e => e.BankCardMonth)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.BankCardName).HasMaxLength(50);

                entity.Property(e => e.BankCardNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BankCardYear).HasMaxLength(2);

                entity.Property(e => e.FkCustumerId).HasColumnName("FK_CustumerId");

                entity.Property(e => e.FkPaymentMethodId).HasColumnName("FK_PaymentMethodId");

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.FkCustumer)
                    .WithMany(p => p.TCustomerBankCard)
                    .HasForeignKey(d => d.FkCustumerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCustomerBankCard_tCustomer");

                entity.HasOne(d => d.FkPaymentMethod)
                    .WithMany(p => p.TCustomerBankCard)
                    .HasForeignKey(d => d.FkPaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tCustomerBankCard_tPaymentMethod");
            });

            modelBuilder.Entity<TDiscountCategory>(entity =>
            {
                entity.HasKey(e => e.AssingnedCategoryId);

                entity.ToTable("tDiscountCategory", "dbo");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkDiscountPlanId).HasColumnName("FK_DiscountPlanId");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TDiscountCategory)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCategory_tCategory");

                entity.HasOne(d => d.FkDiscountPlan)
                    .WithMany(p => p.TDiscountCategory)
                    .HasForeignKey(d => d.FkDiscountPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCategory_tDiscountCoupon");
            });

            modelBuilder.Entity<TDiscountCouponCode>(entity =>
            {
                entity.HasKey(e => e.CodeId)
                    .HasName("PK_tDiscountCode");

                entity.ToTable("tDiscountCouponCode", "dbo");

                entity.HasIndex(e => e.DiscountCode)
                    .HasName("IX_tDiscountCode")
                    .IsUnique();

                entity.Property(e => e.DiscountCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FkDiscountPlanId).HasColumnName("FK_DiscountPlanId");

                entity.HasOne(d => d.FkDiscountPlan)
                    .WithMany(p => p.TDiscountCouponCode)
                    .HasForeignKey(d => d.FkDiscountPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCode_tDiscountCoupon");
            });

            modelBuilder.Entity<TDiscountCouponCodeType>(entity =>
            {
                entity.HasKey(e => e.CodeTypeId);

                entity.ToTable("tDiscountCouponCodeType", "dbo");

                entity.Property(e => e.CodeTypeId).ValueGeneratedNever();

                entity.Property(e => e.CodeTypeTitle)
                    .IsRequired()
                    .HasMaxLength(1500);
            });

            modelBuilder.Entity<TDiscountCustomers>(entity =>
            {
                entity.HasKey(e => e.AssignedCustomerId);

                entity.ToTable("tDiscountCustomers", "dbo");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkDiscountPlanId).HasColumnName("FK_DiscountPlanId");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TDiscountCustomers)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCustomers_tCustomer");

                entity.HasOne(d => d.FkDiscountPlan)
                    .WithMany(p => p.TDiscountCustomers)
                    .HasForeignKey(d => d.FkDiscountPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCustomers_tDiscountCoupon");
            });

            modelBuilder.Entity<TDiscountFreeGoods>(entity =>
            {
                entity.HasKey(e => e.FreeGoodsId);

                entity.ToTable("tDiscountFreeGoods", "dbo");

                entity.Property(e => e.FkDiscountPlanId).HasColumnName("FK_DiscountPlanId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkVarietyId).HasColumnName("FK_VarietyId");

                entity.HasOne(d => d.FkDiscountPlan)
                    .WithMany(p => p.TDiscountFreeGoods)
                    .HasForeignKey(d => d.FkDiscountPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountFreeGoods_tDiscountPlan");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TDiscountFreeGoods)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountFreeGoods_tGoods");

                entity.HasOne(d => d.FkVariety)
                    .WithMany(p => p.TDiscountFreeGoods)
                    .HasForeignKey(d => d.FkVarietyId)
                    .HasConstraintName("FK_tDiscountFreeGoods_tGoodsProvider");
            });

            modelBuilder.Entity<TDiscountGoods>(entity =>
            {
                entity.HasKey(e => e.AssingedGoodsId);

                entity.ToTable("tDiscountGoods", "dbo");

                entity.Property(e => e.FkDiscountPlanId).HasColumnName("FK_DiscountPlanId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkVarietyId).HasColumnName("FK_VarietyId");

                entity.HasOne(d => d.FkDiscountPlan)
                    .WithMany(p => p.TDiscountGoods)
                    .HasForeignKey(d => d.FkDiscountPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountGoods_tDiscountCoupon");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TDiscountGoods)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountGoods_tGoods");

                entity.HasOne(d => d.FkVariety)
                    .WithMany(p => p.TDiscountGoods)
                    .HasForeignKey(d => d.FkVarietyId)
                    .HasConstraintName("FK_tDiscountGoods_tGoodsVariety");
            });

            modelBuilder.Entity<TDiscountPlan>(entity =>
            {
                entity.HasKey(e => e.PlanId)
                    .HasName("PK_tDiscountCoupon");

                entity.ToTable("tDiscountPlan", "dbo");

                entity.Property(e => e.CouponCodePrefix).HasMaxLength(8);

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.EndDateTime)
                    .HasColumnName("EndDateTIme")
                    .HasColumnType("datetime");

                entity.Property(e => e.FkCouponCodeTypeId).HasColumnName("FK_CouponCodeTypeId");

                entity.Property(e => e.FkDiscountRangeTypeId).HasColumnName("FK_DiscountRangeTypeId");

                entity.Property(e => e.FkDiscountTypeId).HasColumnName("FK_DiscountTypeId");

                entity.Property(e => e.FkPlanTypeId).HasColumnName("FK_PlanTypeId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.MaximumDiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MinimumOrderAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.PermittedUseNumberAll).HasColumnName("PermittedUseNumber_All");

                entity.Property(e => e.PermittedUseNumberPerCode).HasColumnName("PermittedUseNumber_PerCode");

                entity.Property(e => e.PermittedUseNumberPerCustomer).HasColumnName("PermittedUseNumber_PerCustomer");

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.FkCouponCodeType)
                    .WithMany(p => p.TDiscountPlan)
                    .HasForeignKey(d => d.FkCouponCodeTypeId)
                    .HasConstraintName("FK_tDiscountCoupon_tDiscountCouponCodeType");

                entity.HasOne(d => d.FkDiscountRangeType)
                    .WithMany(p => p.TDiscountPlan)
                    .HasForeignKey(d => d.FkDiscountRangeTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCoupon_tDiscountRangeType");

                entity.HasOne(d => d.FkDiscountType)
                    .WithMany(p => p.TDiscountPlan)
                    .HasForeignKey(d => d.FkDiscountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCoupon_tDiscountType");

                entity.HasOne(d => d.FkPlanType)
                    .WithMany(p => p.TDiscountPlan)
                    .HasForeignKey(d => d.FkPlanTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountCoupon_tCouponType");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TDiscountPlan)
                    .HasForeignKey(d => d.FkShopId)
                    .HasConstraintName("FK_tDiscountCoupon_tShop");
            });

            modelBuilder.Entity<TDiscountPlanType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_tCouponType");

                entity.ToTable("tDiscountPlanType", "dbo");

                entity.Property(e => e.TypeId).ValueGeneratedNever();

                entity.Property(e => e.TypeTitle)
                    .IsRequired()
                    .HasMaxLength(1500);
            });

            modelBuilder.Entity<TDiscountRangeType>(entity =>
            {
                entity.HasKey(e => e.RangeTypeId)
                    .HasName("PK_tDiscountTargetType");

                entity.ToTable("tDiscountRangeType", "dbo");

                entity.Property(e => e.RangeTypeId).ValueGeneratedNever();

                entity.Property(e => e.RangeTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TDiscountShops>(entity =>
            {
                entity.HasKey(e => e.AssignedShopId);

                entity.ToTable("tDiscountShops", "dbo");

                entity.Property(e => e.AssignedShopId).ValueGeneratedNever();

                entity.Property(e => e.FkDiscountPlanId).HasColumnName("FK_DiscountPlanId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.HasOne(d => d.FkDiscountPlan)
                    .WithMany(p => p.TDiscountShops)
                    .HasForeignKey(d => d.FkDiscountPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountShops_tDiscountCoupon");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TDiscountShops)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDiscountShops_tShop");
            });

            modelBuilder.Entity<TDiscountType>(entity =>
            {
                entity.HasKey(e => e.DiscountTypeId);

                entity.ToTable("tDiscountType", "dbo");

                entity.Property(e => e.DiscountTypeId).ValueGeneratedNever();

                entity.Property(e => e.DiscountTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TDocumentGroup>(entity =>
            {
                entity.HasKey(e => e.DocumentTypeId)
                    .HasName("PK_tDocumentType");

                entity.ToTable("tDocumentGroup", "dbo");

                entity.Property(e => e.DocumentTypeId).ValueGeneratedNever();

                entity.Property(e => e.DocumentTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TDocumentType>(entity =>
            {
                entity.HasKey(e => e.DocumentTypeId)
                    .HasName("PK_tDocument");

                entity.ToTable("tDocumentType", "dbo");

                entity.Property(e => e.DocumentTitle)
                    .IsRequired()
                    .HasMaxLength(1500);

                entity.Property(e => e.FkGroupd).HasColumnName("FK_Groupd");

                entity.Property(e => e.FkPersonId).HasColumnName("FK_PersonId");

                entity.HasOne(d => d.FkGroupdNavigation)
                    .WithMany(p => p.TDocumentType)
                    .HasForeignKey(d => d.FkGroupd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tDocumentType_tDocumentGroup");

                entity.HasOne(d => d.FkPerson)
                    .WithMany(p => p.TDocumentType)
                    .HasForeignKey(d => d.FkPersonId)
                    .HasConstraintName("FK_tDocumentType_tPerson");
            });

            modelBuilder.Entity<TForceUpdate>(entity =>
            {
                entity.HasKey(e => e.ForceUpdateId)
                    .HasName("PK_TForceUpdate");

                entity.ToTable("tForceUpdate", "dbo");

                entity.Property(e => e.ForceUpdateId).ValueGeneratedNever();

                entity.Property(e => e.AndroidVersionName).HasMaxLength(100);

                entity.Property(e => e.AppStoreLink).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DirectDownloadLink).HasMaxLength(500);

                entity.Property(e => e.GooglePlayeLink).HasMaxLength(500);

                entity.Property(e => e.IosVersionName).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(500);
            });

            modelBuilder.Entity<TGoods>(entity =>
            {
                entity.HasKey(e => e.GoodsId);

                entity.ToTable("tGoods", "dbo");

                entity.Property(e => e.DownloadableFileUrl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FkBrandId).HasColumnName("FK_BrandId");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkOwnerId).HasColumnName("FK_OwnerId");

                entity.Property(e => e.FkUnitId).HasColumnName("FK_UnitId");

                entity.Property(e => e.GoodsCode).HasMaxLength(50);

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsAccepted).HasColumnName("isAccepted");

                entity.Property(e => e.LastUpdateDateTime).HasColumnType("datetime");

                entity.Property(e => e.MetaDescription).HasColumnName("Meta_description");

                entity.Property(e => e.MetaKeywords).HasColumnName("Meta_Keywords");

                entity.Property(e => e.MetaTitle).HasColumnName("Meta_Title");

                entity.Property(e => e.PageTitle).HasColumnName("Page_Title");

                entity.Property(e => e.RegisterDate).HasColumnType("date");

                entity.Property(e => e.SerialNumber).HasMaxLength(50);

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.ToBeDisplayed).HasColumnName("toBeDisplayed");

                entity.HasOne(d => d.FkBrand)
                    .WithMany(p => p.TGoods)
                    .HasForeignKey(d => d.FkBrandId)
                    .HasConstraintName("FK_tGoods_tBrand");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TGoods)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoods_tCategory");

                entity.HasOne(d => d.FkOwner)
                    .WithMany(p => p.TGoods)
                    .HasForeignKey(d => d.FkOwnerId)
                    .HasConstraintName("FK_tGoods_tShop");

                entity.HasOne(d => d.FkUnit)
                    .WithMany(p => p.TGoods)
                    .HasForeignKey(d => d.FkUnitId)
                    .HasConstraintName("FK_tGoods_tMeasurementUnit");
            });

            modelBuilder.Entity<TGoodsComment>(entity =>
            {
                entity.HasKey(e => e.CommentId);

                entity.ToTable("tGoodsComment", "dbo");

                entity.Property(e => e.CommentDate).HasColumnType("date");

                entity.Property(e => e.CommentText)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkVarietyId).HasColumnName("FK_VarietyId");

                entity.Property(e => e.IsAccepted).HasColumnName("isAccepted");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TGoodsComment)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsComment_tCustomer");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsComment)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsComment_tGoods");

                entity.HasOne(d => d.FkVariety)
                    .WithMany(p => p.TGoodsComment)
                    .HasForeignKey(d => d.FkVarietyId)
                    .HasConstraintName("FK_tGoodsComment_tGoodsProvider");
            });

            modelBuilder.Entity<TGoodsCommentPoints>(entity =>
            {
                entity.HasKey(e => e.PointId)
                    .HasName("PK_tCommentWeakPoint_1");

                entity.ToTable("tGoodsCommentPoints", "dbo");

                entity.Property(e => e.FkCommentId).HasColumnName("FK_CommentId");

                entity.Property(e => e.PointText)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.PointType).HasComment(@"0:weak
1:strong");

                entity.HasOne(d => d.FkComment)
                    .WithMany(p => p.TGoodsCommentPoints)
                    .HasForeignKey(d => d.FkCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsCommentPoints_tGoodsComment");
            });

            modelBuilder.Entity<TGoodsDocument>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK_tGoodsImages");

                entity.ToTable("tGoodsDocument", "dbo");

                entity.Property(e => e.DocumentUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkVarietyId).HasColumnName("FK_VarietyId");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsDocument)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsImages_tGoods");

                entity.HasOne(d => d.FkVariety)
                    .WithMany(p => p.TGoodsDocument)
                    .HasForeignKey(d => d.FkVarietyId)
                    .HasConstraintName("FK_tGoodsImages_tGoodsColors");
            });

            modelBuilder.Entity<TGoodsLike>(entity =>
            {
                entity.HasKey(e => e.LikeId);

                entity.ToTable("tGoodsLike", "dbo");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.LikeDate).HasColumnType("date");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TGoodsLike)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsLike_tCustomer");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsLike)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsLike_tGoods");
            });

            modelBuilder.Entity<TGoodsProvider>(entity =>
            {
                entity.HasKey(e => e.ProviderId);

                entity.ToTable("tGoodsProvider", "dbo");

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountPercentage).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FinalPrice)
                    .HasColumnType("numeric(20, 2)")
                    .HasComputedColumnSql("(([Price]-[DiscountAmount])+[VATAmount])");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkGuaranteeId).HasColumnName("FK_GuaranteeId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.Price)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ToBeDisplayed).HasColumnName("toBeDisplayed");

                entity.Property(e => e.Vatamount)
                    .HasColumnName("VATAmount")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Vatfree).HasColumnName("VATFree");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsProvider)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsProvider_tGoods");

                entity.HasOne(d => d.FkGuarantee)
                    .WithMany(p => p.TGoodsProvider)
                    .HasForeignKey(d => d.FkGuaranteeId)
                    .HasConstraintName("FK_tGoodsProvider_tGuarantee");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TGoodsProvider)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsProvider_tShop");
            });

            modelBuilder.Entity<TGoodsQueAns>(entity =>
            {
                entity.HasKey(e => e.Qaid);

                entity.ToTable("tGoodsQueAns", "dbo");

                entity.Property(e => e.Qaid).HasColumnName("QAId");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkQaid).HasColumnName("FK_QAId");

                entity.Property(e => e.IsAccepted).HasColumnName("isAccepted");

                entity.Property(e => e.Qadate)
                    .HasColumnName("QADate")
                    .HasColumnType("date");

                entity.Property(e => e.Qatext)
                    .IsRequired()
                    .HasColumnName("QAText")
                    .HasMaxLength(4000);

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TGoodsQueAns)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsQueAns_tCustomer");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsQueAns)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsQueAns_tGoods");

                entity.HasOne(d => d.FkQa)
                    .WithMany(p => p.InverseFkQa)
                    .HasForeignKey(d => d.FkQaid)
                    .HasConstraintName("FK_tGoodsQueAns_tGoodsQueAns1");
            });

            modelBuilder.Entity<TGoodsSpecification>(entity =>
            {
                entity.HasKey(e => e.Gsid);

                entity.ToTable("tGoodsSpecification", "dbo");

                entity.Property(e => e.Gsid).HasColumnName("GSId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkSpecId).HasColumnName("FK_SpecId");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsSpecification)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsSpecification_tGoods");

                entity.HasOne(d => d.FkSpec)
                    .WithMany(p => p.TGoodsSpecification)
                    .HasForeignKey(d => d.FkSpecId)
                    .HasConstraintName("FK_tGoodsSpecification_tSpecification");
            });

            modelBuilder.Entity<TGoodsSpecificationOptions>(entity =>
            {
                entity.HasKey(e => e.SpecOptionId);

                entity.ToTable("tGoodsSpecificationOptions", "dbo");

                entity.Property(e => e.FkGsid).HasColumnName("FK_GSId");

                entity.Property(e => e.FkSpecOptionId).HasColumnName("FK_SpecOptionId");

                entity.HasOne(d => d.FkGs)
                    .WithMany(p => p.TGoodsSpecificationOptions)
                    .HasForeignKey(d => d.FkGsid)
                    .HasConstraintName("FK_tGoodsSpecificationOptions_tGoodsSpecification");

                entity.HasOne(d => d.FkSpecOption)
                    .WithMany(p => p.TGoodsSpecificationOptions)
                    .HasForeignKey(d => d.FkSpecOptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsSpecificationOptions_tSpecificationOptions");
            });

            modelBuilder.Entity<TGoodsSurveyAnswers>(entity =>
            {
                entity.HasKey(e => e.AnsId);

                entity.ToTable("tGoodsSurveyAnswers", "dbo");

                entity.Property(e => e.FkCommentId).HasColumnName("FK_CommentId");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkQuestionId).HasColumnName("FK_QuestionId");

                entity.HasOne(d => d.FkComment)
                    .WithMany(p => p.TGoodsSurveyAnswers)
                    .HasForeignKey(d => d.FkCommentId)
                    .HasConstraintName("FK_tGoodsSurveyAnswers_tGoodsComment");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TGoodsSurveyAnswers)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsSurveyAnswers_tCustomer");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsSurveyAnswers)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsSurveyAnswers_tGoods");

                entity.HasOne(d => d.FkQuestion)
                    .WithMany(p => p.TGoodsSurveyAnswers)
                    .HasForeignKey(d => d.FkQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsSurveyAnswers_tGoodsSurveyQuestions");
            });

            modelBuilder.Entity<TGoodsSurveyQuestions>(entity =>
            {
                entity.HasKey(e => e.QueId);

                entity.ToTable("tGoodsSurveyQuestions", "dbo");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TGoodsSurveyQuestions)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsSurveyQuestions_tCategory");
            });

            modelBuilder.Entity<TGoodsVariety>(entity =>
            {
                entity.HasKey(e => e.VarietyId)
                    .HasName("PK_tGoodsColors");

                entity.ToTable("tGoodsVariety", "dbo");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkProviderId).HasColumnName("FK_ProviderId");

                entity.Property(e => e.FkVariationParameterId).HasColumnName("FK_VariationParameterId");

                entity.Property(e => e.FkVariationParameterValueId).HasColumnName("FK_VariationParameterValueId");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsVariety)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsVariety_tGoods");

                entity.HasOne(d => d.FkProvider)
                    .WithMany(p => p.TGoodsVariety)
                    .HasForeignKey(d => d.FkProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsVariety_tGoodsProvider");

                entity.HasOne(d => d.FkVariationParameter)
                    .WithMany(p => p.TGoodsVariety)
                    .HasForeignKey(d => d.FkVariationParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsVariety_tVariationParameter");

                entity.HasOne(d => d.FkVariationParameterValue)
                    .WithMany(p => p.TGoodsVariety)
                    .HasForeignKey(d => d.FkVariationParameterValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsVariety_tVariationParameterValues");
            });

            modelBuilder.Entity<TGoodsView>(entity =>
            {
                entity.HasKey(e => e.ViewId);

                entity.ToTable("tGoodsView", "dbo");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.IpAddress).HasMaxLength(50);

                entity.Property(e => e.ViewDate).HasColumnType("date");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TGoodsView)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsView_tCustomer");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TGoodsView)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tGoodsView_tGoods");
            });

            modelBuilder.Entity<TGuarantee>(entity =>
            {
                entity.HasKey(e => e.GuaranteeId);

                entity.ToTable("tGuarantee", "dbo");

                entity.Property(e => e.GuaranteeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.IsAccepted).HasColumnName("isAccepted");
            });

            modelBuilder.Entity<THelpArticle>(entity =>
            {
                entity.HasKey(e => e.ArticleId);

                entity.ToTable("tHelpArticle", "dbo");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.FkTopicId).HasColumnName("FK_TopicId");

                entity.Property(e => e.LastUpdateDateTime).HasColumnType("date");

                entity.Property(e => e.Subject).IsRequired();

                entity.HasOne(d => d.FkTopic)
                    .WithMany(p => p.THelpArticle)
                    .HasForeignKey(d => d.FkTopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tHelpArticle_tHelpTopic");
            });

            modelBuilder.Entity<THelpTopic>(entity =>
            {
                entity.HasKey(e => e.TopicId);

                entity.ToTable("tHelpTopic", "dbo");

                entity.Property(e => e.FkTopicId).HasColumnName("FK_TopicId");

                entity.Property(e => e.IconUrl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(d => d.FkTopic)
                    .WithMany(p => p.InverseFkTopic)
                    .HasForeignKey(d => d.FkTopicId)
                    .HasConstraintName("FK_tHelpTopic_tHelpTopic");
            });

            modelBuilder.Entity<TLanguage>(entity =>
            {
                entity.HasKey(e => e.LanguageId);

                entity.ToTable("tLanguage", "dbo");

                entity.Property(e => e.LanguageId).ValueGeneratedNever();

                entity.Property(e => e.IsRtl).HasColumnName("isRtl");

                entity.Property(e => e.JsonFile)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LanguageCode)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.LanguageTitle)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TMeasurementUnit>(entity =>
            {
                entity.HasKey(e => e.UnitId);

                entity.ToTable("tMeasurementUnit", "dbo");

                entity.Property(e => e.UnitTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TMenuItem>(entity =>
            {
                entity.HasKey(e => e.MenuId);

                entity.ToTable("tMenuItem", "dbo");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_tMenuItem_tMenuItem");
            });

            modelBuilder.Entity<TMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.ToTable("tMessage", "dbo");

                entity.Property(e => e.FkInResponseMessageId).HasColumnName("FK_InResponseMessageId");

                entity.Property(e => e.FkSenderId).HasColumnName("FK_SenderId");

                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.Property(e => e.Sms).HasColumnName("SMS");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.FkInResponseMessage)
                    .WithMany(p => p.InverseFkInResponseMessage)
                    .HasForeignKey(d => d.FkInResponseMessageId)
                    .HasConstraintName("FK_tMessage_tMessage");

                entity.HasOne(d => d.FkSender)
                    .WithMany(p => p.TMessage)
                    .HasForeignKey(d => d.FkSenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tMessage_tUser");
            });

            modelBuilder.Entity<TMessageAttachment>(entity =>
            {
                entity.HasKey(e => e.AttachmentId);

                entity.ToTable("tMessageAttachment", "dbo");

                entity.Property(e => e.FileUrl)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FkMessageId).HasColumnName("FK_MessageId");

                entity.Property(e => e.Title).HasMaxLength(150);

                entity.HasOne(d => d.FkMessage)
                    .WithMany(p => p.TMessageAttachment)
                    .HasForeignKey(d => d.FkMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tMessageAttachment_tMessage");
            });

            modelBuilder.Entity<TMessageRecipient>(entity =>
            {
                entity.HasKey(e => e.RecipientId);

                entity.ToTable("tMessageRecipient", "dbo");

                entity.Property(e => e.FkMessageId).HasColumnName("FK_MessageId");

                entity.Property(e => e.FkRecieverId).HasColumnName("FK_RecieverId");

                entity.Property(e => e.ViewDateTime).HasColumnType("datetime");

                entity.Property(e => e.ViewFlag).HasColumnName("viewFlag");

                entity.HasOne(d => d.FkMessage)
                    .WithMany(p => p.TMessageRecipient)
                    .HasForeignKey(d => d.FkMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tMessageRecipient_tMessage");

                entity.HasOne(d => d.FkReciever)
                    .WithMany(p => p.TMessageRecipient)
                    .HasForeignKey(d => d.FkRecieverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tMessageRecipient_tUser");
            });

            modelBuilder.Entity<TNotificationSetting>(entity =>
            {
                entity.HasKey(e => e.NotificationSettingId)
                    .HasName("PK_NotificationSetting");

                entity.ToTable("tNotificationSetting", "dbo");

                entity.Property(e => e.NotificationSettingId).ValueGeneratedNever();

                entity.Property(e => e.Title).HasMaxLength(4000);
            });

            modelBuilder.Entity<TOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.ToTable("tOrder", "dbo");

                entity.Property(e => e.AdAddress)
                    .HasColumnName("AD_Address")
                    .HasMaxLength(250);

                entity.Property(e => e.AdFkCityId).HasColumnName("AD_FK_CityId");

                entity.Property(e => e.AdFkCountryId).HasColumnName("AD_FK_CountryId");

                entity.Property(e => e.AdFkProvinceId).HasColumnName("AD_FK_ProvinceId");

                entity.Property(e => e.AdLocationX).HasColumnName("AD_Location_X");

                entity.Property(e => e.AdLocationY).HasColumnName("AD_Location_Y");

                entity.Property(e => e.AdPostalCode)
                    .HasColumnName("AD_PostalCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AdTransfereeFamily)
                    .HasColumnName("AD_TransfereeFamily")
                    .HasMaxLength(40);

                entity.Property(e => e.AdTransfereeMobile)
                    .HasColumnName("AD_TransfereeMobile")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.AdTransfereeName)
                    .HasColumnName("AD_TransfereeName")
                    .HasMaxLength(30);

                entity.Property(e => e.AdTransfereeTel)
                    .HasColumnName("AD_TransfereeTel")
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ComissionPrice).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FinalPrice)
                    .HasColumnType("numeric(21, 2)")
                    .HasComputedColumnSql("((([Price]+[ShippingCost])+[VATAmount])-[DiscountAmount])");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkDiscountCodeId).HasColumnName("FK_DiscountCodeId");

                entity.Property(e => e.FkOrderStatusId).HasColumnName("FK_OrderStatusId");

                entity.Property(e => e.FkPaymentMethodId).HasColumnName("FK_PaymentMethodId");

                entity.Property(e => e.InitialDateTime).HasColumnType("datetime");

                entity.Property(e => e.PlacedDateTime).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ShippingCost).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TrackingCode)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Vatamount)
                    .HasColumnName("VATAmount")
                    .HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.AdFkCity)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.AdFkCityId)
                    .HasConstraintName("FK_tOrder_tCity");

                entity.HasOne(d => d.AdFkCountry)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.AdFkCountryId)
                    .HasConstraintName("FK_tOrder_tCountry");

                entity.HasOne(d => d.AdFkProvince)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.AdFkProvinceId)
                    .HasConstraintName("FK_tOrder_tProvince");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrder_tCustomer");

                entity.HasOne(d => d.FkDiscountCode)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.FkDiscountCodeId)
                    .HasConstraintName("FK_tOrder_tDiscountCode");

                entity.HasOne(d => d.FkOrderStatus)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.FkOrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrder_tOrderStatus");

                entity.HasOne(d => d.FkPaymentMethod)
                    .WithMany(p => p.TOrder)
                    .HasForeignKey(d => d.FkPaymentMethodId)
                    .HasConstraintName("FK_tOrder_tPaymentType");
            });

            modelBuilder.Entity<TOrderCanceling>(entity =>
            {
                entity.HasKey(e => e.CancelingId)
                    .HasName("PK_tttttttt");

                entity.ToTable("tOrderCanceling", "dbo");

                entity.Property(e => e.CancelDate).HasColumnType("date");

                entity.Property(e => e.Comment).HasMaxLength(4000);

                entity.Property(e => e.FkCancelingReasonId).HasColumnName("FK_CancelingReasonId");

                entity.Property(e => e.FkOrderId).HasColumnName("FK_OrderId");

                entity.Property(e => e.FkOrderItemId).HasColumnName("FK_OrderItemId");

                entity.HasOne(d => d.FkCancelingReason)
                    .WithMany(p => p.TOrderCanceling)
                    .HasForeignKey(d => d.FkCancelingReasonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderCanceling_tCancelingType");

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.TOrderCanceling)
                    .HasForeignKey(d => d.FkOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderCanceling_tOrderCanceling");

                entity.HasOne(d => d.FkOrderItem)
                    .WithMany(p => p.TOrderCanceling)
                    .HasForeignKey(d => d.FkOrderItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderCanceling_tOrderItem");
            });

            modelBuilder.Entity<TOrderCancelingReason>(entity =>
            {
                entity.HasKey(e => e.ReasonId)
                    .HasName("PK_tOrderCanceling");

                entity.ToTable("tOrderCancelingReason", "dbo");

                entity.Property(e => e.ReasonTitle).IsRequired();
            });

            modelBuilder.Entity<TOrderItem>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("tOrderItem", "dbo");

                entity.Property(e => e.ComissionPrice)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DeliveredDate).HasColumnType("date");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DiscountCouponAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FinalPrice).HasComputedColumnSql("((([UnitPrice]*[ItemCount]-[DiscountAmount])+[VATAmount])+[ShippingCost])");

                entity.Property(e => e.FkDiscountCodeId).HasColumnName("FK_DiscountCodeId");

                entity.Property(e => e.FkGoodsId).HasColumnName("FK_GoodsId");

                entity.Property(e => e.FkOrderId).HasColumnName("FK_OrderId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.FkStatusId).HasColumnName("FK_StatusId");

                entity.Property(e => e.FkVarietyId).HasColumnName("FK_VarietyId");

                entity.Property(e => e.ItemCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.ShippingCost)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ShippmentDate).HasColumnType("date");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Vatamount)
                    .HasColumnName("VATAmount")
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.FkDiscountCode)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkDiscountCodeId)
                    .HasConstraintName("FK_tOrderItem_tDiscountCouponCode");

                entity.HasOne(d => d.FkGoods)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkGoodsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderItem_tGoods");

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderItem_tOrderItem");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .HasConstraintName("FK_tOrderItem_tShippingMethod");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderItem_tShop");

                entity.HasOne(d => d.FkStatus)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderItem_tOrderStatus");

                entity.HasOne(d => d.FkVariety)
                    .WithMany(p => p.TOrderItem)
                    .HasForeignKey(d => d.FkVarietyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderItem_tVariety");
            });

            modelBuilder.Entity<TOrderLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK_tORerLog");

                entity.ToTable("tOrderLog", "dbo");

                entity.Property(e => e.FkOrderId).HasColumnName("FK_OrderId");

                entity.Property(e => e.FkOrderItemId).HasColumnName("FK_OrderItemId");

                entity.Property(e => e.FkStatusId).HasColumnName("FK_StatusId");

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");

                entity.Property(e => e.LogComment)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.LogDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.TOrderLog)
                    .HasForeignKey(d => d.FkOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderLog_tOrder");

                entity.HasOne(d => d.FkOrderItem)
                    .WithMany(p => p.TOrderLog)
                    .HasForeignKey(d => d.FkOrderItemId)
                    .HasConstraintName("FK_tOrderLog_tOrderItem");

                entity.HasOne(d => d.FkStatus)
                    .WithMany(p => p.TOrderLog)
                    .HasForeignKey(d => d.FkStatusId)
                    .HasConstraintName("FK_tOrderLog_tOrderStatus");

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.TOrderLog)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderLog_tUser");
            });

            modelBuilder.Entity<TOrderReturning>(entity =>
            {
                entity.HasKey(e => e.ReturningId)
                    .HasName("PK_tOrderREturning");

                entity.ToTable("tOrderReturning", "dbo");

                entity.Property(e => e.FkOrderId).HasColumnName("FK_OrderId");

                entity.Property(e => e.FkOrderItemId).HasColumnName("FK_OrderItemId");

                entity.Property(e => e.FkReturningActionId).HasColumnName("FK_ReturningActionId");

                entity.Property(e => e.FkReturningReasonId).HasColumnName("FK_ReturningReasonId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.Property(e => e.FkStatusId).HasColumnName("FK_StatusId");

                entity.Property(e => e.RegisterDateTime).HasColumnType("datetime");

                entity.Property(e => e.RequestComment).IsRequired();

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.TOrderReturning)
                    .HasForeignKey(d => d.FkOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturning_tOrder");

                entity.HasOne(d => d.FkOrderItem)
                    .WithMany(p => p.TOrderReturning)
                    .HasForeignKey(d => d.FkOrderItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturning_tOrderItem");

                entity.HasOne(d => d.FkReturningAction)
                    .WithMany(p => p.TOrderReturning)
                    .HasForeignKey(d => d.FkReturningActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturning_tReturningType1");

                entity.HasOne(d => d.FkReturningReason)
                    .WithMany(p => p.TOrderReturning)
                    .HasForeignKey(d => d.FkReturningReasonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturning_tReturningType");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TOrderReturning)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .HasConstraintName("FK_tOrderReturning_tShippingMethod");

                entity.HasOne(d => d.FkStatus)
                    .WithMany(p => p.TOrderReturning)
                    .HasForeignKey(d => d.FkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturning_tReturningStatus");
            });

            modelBuilder.Entity<TOrderReturningLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("tOrderReturningLog", "dbo");

                entity.Property(e => e.FkReturningId).HasColumnName("FK_ReturningId");

                entity.Property(e => e.FkStatusId).HasColumnName("FK_StatusId");

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");

                entity.Property(e => e.LogComment)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.LogDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.FkReturning)
                    .WithMany(p => p.TOrderReturningLog)
                    .HasForeignKey(d => d.FkReturningId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturningLog_tOrderReturning");

                entity.HasOne(d => d.FkStatus)
                    .WithMany(p => p.TOrderReturningLog)
                    .HasForeignKey(d => d.FkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturningLog_tReturningStatus");

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.TOrderReturningLog)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tOrderReturningLog_tUser");
            });

            modelBuilder.Entity<TOrderStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("tOrderStatus", "dbo");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.Color)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TPaymentMethod>(entity =>
            {
                entity.HasKey(e => e.MethodId)
                    .HasName("PK_tPaymentType");

                entity.ToTable("tPaymentMethod", "dbo");

                entity.Property(e => e.MethodId).ValueGeneratedNever();

                entity.Property(e => e.MethodImageUrl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MethodTitle)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TPaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.ToTable("tPaymentTransaction", "dbo");

                entity.Property(e => e.TransactionId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.FkCurrencyId).HasColumnName("FK_CurrencyId");

                entity.Property(e => e.FkOrderId).HasColumnName("FK_OrderId");

                entity.Property(e => e.FkPaymentMethodId).HasColumnName("FK_PaymentMethodId");

                entity.Property(e => e.FkPlanId).HasColumnName("FK_PlanId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.PayerId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TempDiscountCode)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkCurrency)
                    .WithMany(p => p.TPaymentTransaction)
                    .HasForeignKey(d => d.FkCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tPaymentTransaction_tCurrency");

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.TPaymentTransaction)
                    .HasForeignKey(d => d.FkOrderId)
                    .HasConstraintName("FK_tPaymentTransaction_tOrder");

                entity.HasOne(d => d.FkPaymentMethod)
                    .WithMany(p => p.TPaymentTransaction)
                    .HasForeignKey(d => d.FkPaymentMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tPaymentTransaction_tPaymentMethod");

                entity.HasOne(d => d.FkPlan)
                    .WithMany(p => p.TPaymentTransaction)
                    .HasForeignKey(d => d.FkPlanId)
                    .HasConstraintName("FK_tPaymentTransaction_tShopPlan");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TPaymentTransaction)
                    .HasForeignKey(d => d.FkShopId)
                    .HasConstraintName("FK_tPaymentTransaction_tShop");
            });

            modelBuilder.Entity<TPerson>(entity =>
            {
                entity.HasKey(e => e.PersonTypeId);

                entity.ToTable("tPerson", "dbo");

                entity.Property(e => e.PersonTypeId).ValueGeneratedNever();

                entity.Property(e => e.PersonTypeTitle)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TPopupItem>(entity =>
            {
                entity.HasKey(e => e.PopupId);

                entity.ToTable("tPopupItem", "dbo");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkTDiscountPlanId).HasColumnName("FK_tDiscountPlanId");

                entity.Property(e => e.PopupImageUrl).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TPopupItem)
                    .HasForeignKey(d => d.FkCategoryId)
                    .HasConstraintName("FK_tPopupItem_tCategory");

                entity.HasOne(d => d.FkTDiscountPlan)
                    .WithMany(p => p.TPopupItem)
                    .HasForeignKey(d => d.FkTDiscountPlanId)
                    .HasConstraintName("FK_tPopupItem_tDiscountPlan");
            });

            modelBuilder.Entity<TProvince>(entity =>
            {
                entity.HasKey(e => e.ProvinceId);

                entity.ToTable("tProvince", "dbo");

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.PName)
                    .IsRequired()
                    .HasColumnName("p_name")
                    .HasMaxLength(50);

                entity.Property(e => e.ProvinceName)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TProvince)
                    .HasForeignKey(d => d.FkCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tProvince_tCountry");
            });

            modelBuilder.Entity<TRecommendation>(entity =>
            {
                entity.HasKey(e => e.RecommendationId);

                entity.ToTable("tRecommendation", "dbo");

                entity.Property(e => e.FkCollectionItemTypeId).HasColumnName("FK_CollectionItemTypeId");

                entity.Property(e => e.FkItemType).HasColumnName("FK_ItemType");

                entity.Property(e => e.XCollectionItemIds)
                    .IsRequired()
                    .HasColumnName("xCollectionItemIds");

                entity.Property(e => e.XItemId).HasColumnName("xItemId");

                entity.HasOne(d => d.FkCollectionItemType)
                    .WithMany(p => p.TRecommendation)
                    .HasForeignKey(d => d.FkCollectionItemTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tRecommendation_tRecommendationCollectionType");

                entity.HasOne(d => d.FkItemTypeNavigation)
                    .WithMany(p => p.TRecommendation)
                    .HasForeignKey(d => d.FkItemType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tRecommendation_tRecommendationItemType");
            });

            modelBuilder.Entity<TRecommendationCollectionType>(entity =>
            {
                entity.HasKey(e => e.CollectionTypeId)
                    .HasName("PK_tRecommendationItemType");

                entity.ToTable("tRecommendationCollectionType", "dbo");

                entity.Property(e => e.CollectionTypeId).ValueGeneratedNever();

                entity.Property(e => e.CollectionTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TRecommendationItemType>(entity =>
            {
                entity.HasKey(e => e.ItemCode)
                    .HasName("PK_tRecommendationItemType_1");

                entity.ToTable("tRecommendationItemType", "dbo");

                entity.Property(e => e.ItemCode).ValueGeneratedNever();

                entity.Property(e => e.ItemType)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TReturningAction>(entity =>
            {
                entity.HasKey(e => e.ReturningTypeId)
                    .HasName("PK_tReturningType_1");

                entity.ToTable("tReturningAction", "dbo");

                entity.Property(e => e.ReturningTypeId).ValueGeneratedNever();

                entity.Property(e => e.ReturningTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TReturningReason>(entity =>
            {
                entity.HasKey(e => e.ReasonId)
                    .HasName("PK_tReturningType");

                entity.ToTable("tReturningReason", "dbo");

                entity.Property(e => e.ReasonTitle).IsRequired();
            });

            modelBuilder.Entity<TReturningStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("tReturningStatus", "dbo");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TSetting>(entity =>
            {
                entity.HasKey(e => e.SettingId);

                entity.ToTable("tSetting", "dbo");

                entity.Property(e => e.SettingId).ValueGeneratedNever();

                entity.Property(e => e.AboutUs).IsRequired();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(1500);

                entity.Property(e => e.AparatUrl)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerLoginPageBackgroundImage).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FacebookUrl)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Fax)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FooterCustomerRights)
                    .IsRequired()
                    .HasColumnName("Footer_CustomerRights");

                entity.Property(e => e.FooterMaxItem).HasColumnName("Footer_MaxItem");

                entity.Property(e => e.FooterMaxItemPerColumn).HasColumnName("Footer_MaxItemPerColumn");

                entity.Property(e => e.FooterPrivacyPolicy)
                    .IsRequired()
                    .HasColumnName("Footer_PrivacyPolicy");

                entity.Property(e => e.FooterTermOfSale)
                    .IsRequired()
                    .HasColumnName("Footer_TermOfSale");

                entity.Property(e => e.FooterTermOfUser)
                    .IsRequired()
                    .HasColumnName("Footer_TermOfUser");

                entity.Property(e => e.FooterWarrantyPolicy)
                    .IsRequired()
                    .HasColumnName("Footer_WarrantyPolicy");

                entity.Property(e => e.HelpPageBackgroundImage).HasMaxLength(500);

                entity.Property(e => e.InstagramUrl)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LinkedinUrl)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LogoUrlLoginPage)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LogoUrlShopFooter)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LogoUrlShopHeader)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MetaDescription)
                    .IsRequired()
                    .HasColumnName("Meta_Description");

                entity.Property(e => e.MetaKeyword)
                    .IsRequired()
                    .HasColumnName("Meta_Keyword");

                entity.Property(e => e.MetaTitle)
                    .IsRequired()
                    .HasColumnName("Meta_Title");

                entity.Property(e => e.PageTitle)
                    .IsRequired()
                    .HasColumnName("Page_Title");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShopDefaultBannerImageUrl)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ShopTitle)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShortDescription).IsRequired();

                entity.Property(e => e.SmsApiKey)
                    .IsRequired()
                    .HasColumnName("SMS_ApiKey")
                    .HasMaxLength(550);

                entity.Property(e => e.SmsPassword)
                    .IsRequired()
                    .HasColumnName("SMS_Password")
                    .HasMaxLength(50);

                entity.Property(e => e.SmsSender)
                    .IsRequired()
                    .HasColumnName("SMS_Sender")
                    .HasMaxLength(550);

                entity.Property(e => e.SmsUrl)
                    .IsRequired()
                    .HasColumnName("SMS_Url")
                    .HasMaxLength(550);

                entity.Property(e => e.SmsUsername)
                    .IsRequired()
                    .HasColumnName("SMS_Username")
                    .HasMaxLength(50);

                entity.Property(e => e.SmtpHost)
                    .IsRequired()
                    .HasColumnName("smtp_Host")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SmtpPassword)
                    .IsRequired()
                    .HasColumnName("smtp_Password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SmtpUsername)
                    .IsRequired()
                    .HasColumnName("smtp_Username")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupportPhone)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SysAutoActiveBrand).HasColumnName("sys_AutoActiveBrand");

                entity.Property(e => e.SysAutoActiveGoods).HasColumnName("sys_AutoActiveGoods");

                entity.Property(e => e.SysAutoActiveGuarantee).HasColumnName("sys_AutoActiveGuarantee");

                entity.Property(e => e.SysDisplayCategoriesWithoutGoods).HasColumnName("sys_DisplayCategoriesWithoutGoods");

                entity.Property(e => e.TelegramUrl)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TwitterUrl)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TSettingPayment>(entity =>
            {
                entity.HasKey(e => e.SettingPaymentId);

                entity.ToTable("tSetting_Payment", "dbo");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PaymentTitle).HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<TShippingMethod>(entity =>
            {
                entity.ToTable("tShippingMethod", "dbo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Image).HasMaxLength(50);

                entity.Property(e => e.ShippingMethodTitle)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<TShippingMethodAreaCode>(entity =>
            {
                entity.HasKey(e => e.PostAreaCodeId);

                entity.ToTable("tShippingMethodAreaCode", "dbo");

                entity.Property(e => e.FkCityId).HasColumnName("FK_CityId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.TShippingMethodAreaCode)
                    .HasForeignKey(d => d.FkCityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShippingMethodAreaCode_tCity");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TShippingMethodAreaCode)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShippingMethodAreaCode_tShippingMethod");
            });

            modelBuilder.Entity<TShippingOnCity>(entity =>
            {
                entity.ToTable("tShippingOnCity", "dbo");

                entity.Property(e => e.FkCityId).HasColumnName("FK_CityId");

                entity.Property(e => e.FkProviceId).HasColumnName("FK_ProviceId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.Property(e => e.ShippingPriceFewerBaseWeight).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ShippingPriceMoreBaseWeight).HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.TShippingOnCity)
                    .HasForeignKey(d => d.FkCityId)
                    .HasConstraintName("FK_tShippingOnCity_tCity");

                entity.HasOne(d => d.FkProvice)
                    .WithMany(p => p.TShippingOnCity)
                    .HasForeignKey(d => d.FkProviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShippingOnCity_tProvince");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TShippingOnCity)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShippingOnCity_tShippingMethod");
            });

            modelBuilder.Entity<TShippingOnCountry>(entity =>
            {
                entity.ToTable("tShippingOnCountry", "dbo");

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.Property(e => e.ShippingPriceFewerBaseWeight).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ShippingPriceMoreBaseWeight).HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TShippingOnCountry)
                    .HasForeignKey(d => d.FkCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShippingOnCountry_tCountry");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TShippingOnCountry)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShippingOnCountry_tShippingMethod");
            });

            modelBuilder.Entity<TShop>(entity =>
            {
                entity.HasKey(e => e.ShopId);

                entity.ToTable("tShop", "dbo");

                entity.Property(e => e.BankAccountNumber)
                    .HasColumnName("Bank_AccountNumber")
                    .HasMaxLength(50);

                entity.Property(e => e.BankBeneficiaryName)
                    .HasColumnName("Bank_BeneficiaryName")
                    .HasMaxLength(50);

                entity.Property(e => e.BankBranch)
                    .HasColumnName("Bank_Branch")
                    .HasMaxLength(50);

                entity.Property(e => e.BankIban)
                    .HasColumnName("Bank_IBAN")
                    .HasMaxLength(50);

                entity.Property(e => e.BankName)
                    .HasColumnName("Bank_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.BankSwiftCode)
                    .HasColumnName("Bank_SwiftCode")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(150);

                entity.Property(e => e.Credit).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.FkCityId).HasColumnName("FK_CityId");

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.FkCurrencyId).HasColumnName("FK_CurrencyId");

                entity.Property(e => e.FkPersonId).HasColumnName("FK_PersonId");

                entity.Property(e => e.FkPlanId).HasColumnName("FK_PlanId");

                entity.Property(e => e.FkProvinceId).HasColumnName("FK_ProvinceId");

                entity.Property(e => e.FkStatusId).HasColumnName("FK_StatusId");

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.GoodsIncludedVat).HasColumnName("GoodsIncludedVAT");

                entity.Property(e => e.LocationX).HasColumnName("Location_X");

                entity.Property(e => e.LocationY).HasColumnName("Location_Y");

                entity.Property(e => e.LogoImage)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProfileImage)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RegisteryDateTime).HasColumnType("datetime");

                entity.Property(e => e.RentPerMonth).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ShopShippingCode).HasMaxLength(50);

                entity.Property(e => e.StoreName).IsRequired();

                entity.Property(e => e.TaxRegistrationNumber).HasMaxLength(50);

                entity.Property(e => e.TermCondition).HasColumnName("Term_Condition");

                entity.Property(e => e.VendorUrlid)
                    .HasColumnName("VendorURLID")
                    .HasMaxLength(50);

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.TShop)
                    .HasForeignKey(d => d.FkCityId)
                    .HasConstraintName("FK_tShop_tCity");

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TShop)
                    .HasForeignKey(d => d.FkCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShop_tCountry");

                entity.HasOne(d => d.FkPerson)
                    .WithMany(p => p.TShop)
                    .HasForeignKey(d => d.FkPersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShop_tPerson");

                entity.HasOne(d => d.FkPlan)
                    .WithMany(p => p.TShop)
                    .HasForeignKey(d => d.FkPlanId)
                    .HasConstraintName("FK_tShop_tPlan");

                entity.HasOne(d => d.FkProvince)
                    .WithMany(p => p.TShop)
                    .HasForeignKey(d => d.FkProvinceId)
                    .HasConstraintName("FK_tShop_tProvince");

                entity.HasOne(d => d.FkStatus)
                    .WithMany(p => p.TShop)
                    .HasForeignKey(d => d.FkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShop_tShopStatus");
            });

            modelBuilder.Entity<TShopActivityCity>(entity =>
            {
                entity.ToTable("tShopActivityCity", "dbo");

                entity.Property(e => e.FkCityId).HasColumnName("FK_CityId");

                entity.Property(e => e.FkProviceId).HasColumnName("FK_ProviceId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.ShippingPriceFewerBaseWeight).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ShippingPriceMoreBaseWeight).HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.FkCity)
                    .WithMany(p => p.TShopActivityCity)
                    .HasForeignKey(d => d.FkCityId)
                    .HasConstraintName("FK_tShopActivityCity_tCity");

                entity.HasOne(d => d.FkProvice)
                    .WithMany(p => p.TShopActivityCity)
                    .HasForeignKey(d => d.FkProviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopActivityCity_tProvince");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TShopActivityCity)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopActivityCity_tShippingMethod");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopActivityCity)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopActivityCity_tShop");
            });

            modelBuilder.Entity<TShopActivityCountry>(entity =>
            {
                entity.ToTable("tShopActivityCountry", "dbo");

                entity.Property(e => e.FkCountryId).HasColumnName("FK_CountryId");

                entity.Property(e => e.FkShippingMethodId).HasColumnName("FK_ShippingMethodId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.HasOne(d => d.FkCountry)
                    .WithMany(p => p.TShopActivityCountry)
                    .HasForeignKey(d => d.FkCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopActivityCountry_tCountry");

                entity.HasOne(d => d.FkShippingMethod)
                    .WithMany(p => p.TShopActivityCountry)
                    .HasForeignKey(d => d.FkShippingMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopActivityCountry_tShippingMethod");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopActivityCountry)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopActivityCountry_tShop");
            });

            modelBuilder.Entity<TShopCategory>(entity =>
            {
                entity.HasKey(e => e.ShopCategoryId);

                entity.ToTable("tShopCategory", "dbo");

                entity.Property(e => e.ContractCommissionFee).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TShopCategory)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopCategory_tCategory");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopCategory)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopCategory_tShop");
            });

            modelBuilder.Entity<TShopComment>(entity =>
            {
                entity.HasKey(e => e.CommentId);

                entity.ToTable("tShopComment", "dbo");

                entity.Property(e => e.CommentDate).HasColumnType("date");

                entity.Property(e => e.CommentText)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CommentTitle)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.IsAccepted).HasColumnName("isAccepted");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TShopComment)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopComment_tCustomer");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopComment)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopComment_tShop");
            });

            modelBuilder.Entity<TShopCommentLike>(entity =>
            {
                entity.HasKey(e => e.LikeId);

                entity.ToTable("tShopCommentLike", "dbo");

                entity.Property(e => e.FkCommentId).HasColumnName("FK_CommentId");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.LikeDate).HasColumnType("date");

                entity.HasOne(d => d.FkComment)
                    .WithMany(p => p.TShopCommentLike)
                    .HasForeignKey(d => d.FkCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopCommentLike_tShopComment");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TShopCommentLike)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopCommentLike_tCustomer");
            });

            modelBuilder.Entity<TShopFiles>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.ToTable("tShopFiles", "dbo");

                entity.Property(e => e.FileUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FkDocumentTypeId).HasColumnName("FK_DocumentTypeId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.HasOne(d => d.FkDocumentType)
                    .WithMany(p => p.TShopFiles)
                    .HasForeignKey(d => d.FkDocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopFiles_tDocument");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopFiles)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopFiles_tShopFiles");
            });

            modelBuilder.Entity<TShopPlan>(entity =>
            {
                entity.HasKey(e => e.PlanId)
                    .HasName("PK_tPlan");

                entity.ToTable("tShopPlan", "dbo");

                entity.Property(e => e.MaxCategory).HasColumnName("Max_Category");

                entity.Property(e => e.MaxProduct).HasColumnName("Max_Product");

                entity.Property(e => e.PlanTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.RentPerMonth).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<TShopPlanExclusive>(entity =>
            {
                entity.HasKey(e => e.ShopPlanExclusiveId);

                entity.ToTable("tShopPlanExclusive", "dbo");

                entity.Property(e => e.FkPlanId).HasColumnName("FK_PlanId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.HasOne(d => d.FkPlan)
                    .WithMany(p => p.TShopPlanExclusive)
                    .HasForeignKey(d => d.FkPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopPlanExclusive_tShopPlan");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopPlanExclusive)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopPlanExclusive_tShop");
            });

            modelBuilder.Entity<TShopSlider>(entity =>
            {
                entity.HasKey(e => e.SliderId);

                entity.ToTable("tShopSlider", "dbo");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopSlider)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopSlider_tShop");
            });

            modelBuilder.Entity<TShopStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("tShopStatus", "dbo");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasMaxLength(2500);

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(2000);
            });

            modelBuilder.Entity<TShopSurveyAnswers>(entity =>
            {
                entity.HasKey(e => e.AnsId);

                entity.ToTable("tShopSurveyAnswers", "dbo");

                entity.Property(e => e.FkCommentId).HasColumnName("FK_CommentId");

                entity.Property(e => e.FkCustomerId).HasColumnName("FK_CustomerId");

                entity.Property(e => e.FkOrderItemId).HasColumnName("FK_OrderItemId");

                entity.Property(e => e.FkQuestionId).HasColumnName("FK_QuestionId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.HasOne(d => d.FkComment)
                    .WithMany(p => p.TShopSurveyAnswers)
                    .HasForeignKey(d => d.FkCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopSurveyAnswers_tGoodsComment");

                entity.HasOne(d => d.FkCustomer)
                    .WithMany(p => p.TShopSurveyAnswers)
                    .HasForeignKey(d => d.FkCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopSurveyAnswers_tCustomer");

                entity.HasOne(d => d.FkOrderItem)
                    .WithMany(p => p.TShopSurveyAnswers)
                    .HasForeignKey(d => d.FkOrderItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopSurveyAnswers_tOrderItem");

                entity.HasOne(d => d.FkQuestion)
                    .WithMany(p => p.TShopSurveyAnswers)
                    .HasForeignKey(d => d.FkQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopSurveyAnswers_tShopSurveyQuestions");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopSurveyAnswers)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopSurveyAnswers_tShop");
            });

            modelBuilder.Entity<TShopSurveyQuestions>(entity =>
            {
                entity.HasKey(e => e.QueId)
                    .HasName("PK_tShopSurverQuestions");

                entity.ToTable("tShopSurveyQuestions", "dbo");

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<TShopWithdrawalRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("tShopWithdrawalRequest", "dbo");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DocumentUrl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.RequestDate).HasColumnType("date");

                entity.Property(e => e.RequestText).HasMaxLength(4000);

                entity.Property(e => e.ResponseText).HasMaxLength(4000);

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TShopWithdrawalRequest)
                    .HasForeignKey(d => d.FkShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tShopWithdrawalRequest_tShop");
            });

            modelBuilder.Entity<TSmstext>(entity =>
            {
                entity.HasKey(e => e.SmsId);

                entity.ToTable("tSMSText", "dbo");

                entity.Property(e => e.SmsId)
                    .HasColumnName("smsId")
                    .ValueGeneratedNever();

                entity.Property(e => e.SmsText)
                    .IsRequired()
                    .HasColumnName("smsText");

                entity.Property(e => e.Token).HasMaxLength(4000);
            });

            modelBuilder.Entity<TSpecification>(entity =>
            {
                entity.HasKey(e => e.SpecId);

                entity.ToTable("tSpecification", "dbo");

                entity.Property(e => e.FkSpecGroupId).HasColumnName("FK_SpecGroupId");

                entity.Property(e => e.IsKeySpec).HasColumnName("isKeySpec");

                entity.Property(e => e.IsRequired).HasColumnName("isRequired");

                entity.Property(e => e.SpecTitle)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.FkSpecGroup)
                    .WithMany(p => p.TSpecification)
                    .HasForeignKey(d => d.FkSpecGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tSpecification_tSpecificationGroup");
            });

            modelBuilder.Entity<TSpecificationGroup>(entity =>
            {
                entity.HasKey(e => e.SpecGroupId);

                entity.ToTable("tSpecificationGroup", "dbo");

                entity.Property(e => e.SpecGroupTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TSpecificationOptions>(entity =>
            {
                entity.HasKey(e => e.OptionId);

                entity.ToTable("tSpecificationOptions", "dbo");

                entity.Property(e => e.FkSpecId).HasColumnName("FK_SpecId");

                entity.Property(e => e.OptionTitle).IsRequired();

                entity.HasOne(d => d.FkSpec)
                    .WithMany(p => p.TSpecificationOptions)
                    .HasForeignKey(d => d.FkSpecId)
                    .HasConstraintName("FK_tSpecificationOptions_tSpecification");
            });

            modelBuilder.Entity<TStockOperation>(entity =>
            {
                entity.HasKey(e => e.OperationId);

                entity.ToTable("tStockOperation", "dbo");

                entity.Property(e => e.FkOperationTypeId).HasColumnName("FK_OperationTypeId");

                entity.Property(e => e.FkOrderItem).HasColumnName("FK_OrderItem");

                entity.Property(e => e.FkStockId).HasColumnName("FK_StockId");

                entity.Property(e => e.OperationComment).HasMaxLength(550);

                entity.Property(e => e.OperationDate).HasColumnType("date");

                entity.Property(e => e.SaleUnitPrice).HasColumnType("numeric(18, 2)");

                entity.HasOne(d => d.FkOperationType)
                    .WithMany(p => p.TStockOperation)
                    .HasForeignKey(d => d.FkOperationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tStockOperation_tStockOperationType");

                entity.HasOne(d => d.FkOrderItemNavigation)
                    .WithMany(p => p.TStockOperation)
                    .HasForeignKey(d => d.FkOrderItem)
                    .HasConstraintName("FK_tStockOperation_tOrderItem");

                entity.HasOne(d => d.FkStock)
                    .WithMany(p => p.TStockOperation)
                    .HasForeignKey(d => d.FkStockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tStockOperation_tGoodsProvider");
            });

            modelBuilder.Entity<TStockOperationType>(entity =>
            {
                entity.HasKey(e => e.OperationTypeId);

                entity.ToTable("tStockOperationType", "dbo");

                entity.Property(e => e.OperationTypeId).ValueGeneratedNever();

                entity.Property(e => e.MiniTitle).HasMaxLength(4000);

                entity.Property(e => e.OperationTypeEffect)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.OperationTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TTransactionStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.ToTable("tTransactionStatus", "dbo");

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TTransactionType>(entity =>
            {
                entity.HasKey(e => e.TransactionTypeId);

                entity.ToTable("tTransactionType", "dbo");

                entity.Property(e => e.TransactionTypeId).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasMaxLength(4000);

                entity.Property(e => e.Kind)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionTypeTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_tUSer");

                entity.ToTable("tUser", "dbo");

                entity.Property(e => e.UserId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.FkCustumerId).HasColumnName("FK_CustumerId");

                entity.Property(e => e.FkShopId).HasColumnName("FK_ShopId");

                entity.Property(e => e.FkUserGroupId).HasColumnName("FK_UserGroupId");

                entity.Property(e => e.LastLoginDatetime).HasColumnType("datetime");

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.PasswordSalt).IsRequired();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.FkCustumer)
                    .WithMany(p => p.TUser)
                    .HasForeignKey(d => d.FkCustumerId)
                    .HasConstraintName("FK_tUSer_tCustomer");

                entity.HasOne(d => d.FkShop)
                    .WithMany(p => p.TUser)
                    .HasForeignKey(d => d.FkShopId)
                    .HasConstraintName("FK_tUser_tShop");

                entity.HasOne(d => d.FkUserGroup)
                    .WithMany(p => p.TUser)
                    .HasForeignKey(d => d.FkUserGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tUser_tUserGroup");
            });

            modelBuilder.Entity<TUserAccessControl>(entity =>
            {
                entity.HasKey(e => e.UserAccessControlId);

                entity.ToTable("tUserAccessControl", "dbo");

                entity.Property(e => e.FkMenuItemId).HasColumnName("FK_MenuItemId");

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");

                entity.HasOne(d => d.FkMenuItem)
                    .WithMany(p => p.TUserAccessControl)
                    .HasForeignKey(d => d.FkMenuItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tUserAccessControl_tMenuItem");

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.TUserAccessControl)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tUserAccessControl_tUser");
            });

            modelBuilder.Entity<TUserGroup>(entity =>
            {
                entity.HasKey(e => e.UserGroupId);

                entity.ToTable("tUserGroup", "dbo");

                entity.Property(e => e.UserGroupId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.UserGroupTitle)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TUserTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.ToTable("tUserTransaction", "dbo");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Comment).HasMaxLength(4000);

                entity.Property(e => e.FkApprovalStatusId).HasColumnName("FK_ApprovalStatusId");

                entity.Property(e => e.FkOrderId).HasColumnName("FK_OrderId");

                entity.Property(e => e.FkOrderItemId).HasColumnName("FK_OrderItemId");

                entity.Property(e => e.FkReturningId).HasColumnName("FK_ReturningId");

                entity.Property(e => e.FkTransactionTypeId).HasColumnName("FK_TransactionTypeId");

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserId");

                entity.Property(e => e.TransactionDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.FkApprovalStatus)
                    .WithMany(p => p.TUserTransaction)
                    .HasForeignKey(d => d.FkApprovalStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tUserTransaction_tTransactionStatus");

                entity.HasOne(d => d.FkOrder)
                    .WithMany(p => p.TUserTransaction)
                    .HasForeignKey(d => d.FkOrderId)
                    .HasConstraintName("FK_tUserTransaction_tOrder");

                entity.HasOne(d => d.FkOrderItem)
                    .WithMany(p => p.TUserTransaction)
                    .HasForeignKey(d => d.FkOrderItemId)
                    .HasConstraintName("FK_tUserTransaction_tOrderItem");

                entity.HasOne(d => d.FkReturning)
                    .WithMany(p => p.TUserTransaction)
                    .HasForeignKey(d => d.FkReturningId)
                    .HasConstraintName("FK_tUserTransaction_tOrderReturning");

                entity.HasOne(d => d.FkTransactionType)
                    .WithMany(p => p.TUserTransaction)
                    .HasForeignKey(d => d.FkTransactionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tUserTransaction_tTransactionType");

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.TUserTransaction)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tUserTransaction_tUser");
            });

            modelBuilder.Entity<TVariationParameter>(entity =>
            {
                entity.HasKey(e => e.ParameterId);

                entity.ToTable("tVariationParameter", "dbo");

                entity.Property(e => e.ParameterTitle)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            modelBuilder.Entity<TVariationParameterValues>(entity =>
            {
                entity.HasKey(e => e.ValueId);

                entity.ToTable("tVariationParameterValues", "dbo");

                entity.Property(e => e.FkParameterId).HasColumnName("FK_ParameterId");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.FkParameter)
                    .WithMany(p => p.TVariationParameterValues)
                    .HasForeignKey(d => d.FkParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tVariationParameterValues_tVariationParameter");
            });

            modelBuilder.Entity<TVariationPerCategory>(entity =>
            {
                entity.ToTable("tVariationPerCategory", "dbo");

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkParameterId).HasColumnName("FK_ParameterId");

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.TVariationPerCategory)
                    .HasForeignKey(d => d.FkCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tVariationPerCategory_tCategory");

                entity.HasOne(d => d.FkParameter)
                    .WithMany(p => p.TVariationPerCategory)
                    .HasForeignKey(d => d.FkParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tVariationPerCategory_tVariationParameter");
            });

            modelBuilder.Entity<TVerification>(entity =>
            {
                entity.HasKey(e => e.VarificationId);

                entity.ToTable("tVerification", "dbo");

                entity.Property(e => e.ControlDateTime).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.InsertDateTime).HasColumnType("datetime");

                entity.Property(e => e.MobileNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<TWorkingShift>(entity =>
            {
                entity.HasKey(e => e.ShiftId);

                entity.ToTable("tWorkingShift", "dbo");
            });

            modelBuilder.Entity<WebCollectionType>(entity =>
            {
                entity.HasKey(e => e.CollectionTypeId);

                entity.ToTable("Web_CollectionType", "dbo");

                entity.Property(e => e.CollectionTypeId).ValueGeneratedNever();

                entity.Property(e => e.CollectionTypeTitle)
                    .IsRequired()
                    .HasMaxLength(650);
            });

            modelBuilder.Entity<WebIndexModuleList>(entity =>
            {
                entity.HasKey(e => e.IModuleId);

                entity.ToTable("Web_IndexModuleList", "dbo");

                entity.Property(e => e.IModuleId).HasColumnName("iModuleId");

                entity.Property(e => e.BackgroundImageUrl)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkModuleId).HasColumnName("FK_ModuleId");

                entity.Property(e => e.ModuleTitle).HasMaxLength(800);

                entity.Property(e => e.SelectedHeight)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.WebIndexModuleList)
                    .HasForeignKey(d => d.FkCategoryId)
                    .HasConstraintName("FK_Web_IndexModuleList_tCategory");

                entity.HasOne(d => d.FkModule)
                    .WithMany(p => p.WebIndexModuleList)
                    .HasForeignKey(d => d.FkModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Web_IndexModuleList_Web_Module");
            });

            modelBuilder.Entity<WebModule>(entity =>
            {
                entity.HasKey(e => e.ModuleId);

                entity.ToTable("Web_Module", "dbo");

                entity.Property(e => e.ModuleId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.ModuleTitle)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<WebModuleCollections>(entity =>
            {
                entity.HasKey(e => e.CollectionId);

                entity.ToTable("Web_ModuleCollections", "dbo");

                entity.Property(e => e.CriteriaFrom).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CriteriaTo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FkCollectionTypeId).HasColumnName("FK_CollectionTypeId");

                entity.Property(e => e.FkIModuleId).HasColumnName("FK_iModuleId");

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.Property(e => e.LinkUrl).HasMaxLength(2550);

                entity.Property(e => e.ResponsiveImageUrl).IsUnicode(false);

                entity.Property(e => e.XitemIds)
                    .HasColumnName("XItemIds")
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkCollectionType)
                    .WithMany(p => p.WebModuleCollections)
                    .HasForeignKey(d => d.FkCollectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Web_ModuleCollections_Web_CollectionType");

                entity.HasOne(d => d.FkIModule)
                    .WithMany(p => p.WebModuleCollections)
                    .HasForeignKey(d => d.FkIModuleId)
                    .HasConstraintName("FK_Web_ModuleCollections_Web_IndexModuleList");
            });

            modelBuilder.Entity<WebSlider>(entity =>
            {
                entity.HasKey(e => e.SliderId);

                entity.ToTable("Web_Slider", "dbo");

                entity.Property(e => e.CriteriaFrom).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CriteriaTo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ExternalLinkUrl).HasMaxLength(2550);

                entity.Property(e => e.FkCategoryId).HasColumnName("FK_CategoryId");

                entity.Property(e => e.FkCollectionTypeId).HasColumnName("FK_CollectionTypeId");

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ResponsiveImageUrl).IsUnicode(false);

                entity.Property(e => e.XitemIds)
                    .HasColumnName("XItemIds")
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkCategory)
                    .WithMany(p => p.WebSlider)
                    .HasForeignKey(d => d.FkCategoryId)
                    .HasConstraintName("FK_Web_Slider_tCategory");

                entity.HasOne(d => d.FkCollectionType)
                    .WithMany(p => p.WebSlider)
                    .HasForeignKey(d => d.FkCollectionTypeId)
                    .HasConstraintName("FK_Web_Slider_Web_CollectionType");
            });

            modelBuilder.Entity<WebSliderItems>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("Web_SliderItems", "dbo");

                entity.Property(e => e.FkSliderId).HasColumnName("FK_SliderId");

                entity.Property(e => e.XitemId).HasColumnName("XItemId");

                entity.HasOne(d => d.FkSlider)
                    .WithMany(p => p.WebSliderItems)
                    .HasForeignKey(d => d.FkSliderId)
                    .HasConstraintName("FK_Web_SliderItems_Web_Slider");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
