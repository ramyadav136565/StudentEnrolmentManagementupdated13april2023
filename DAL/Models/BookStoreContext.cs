using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace DAL.Models
{
    public partial class BookStoreContext : DbContext
    {
      

        private IConfiguration _config;
      
        public BookStoreContext(IConfiguration config)
        {
            _config = config;
               
        }


        public BookStoreContext(DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookAllocation> BookAllocations { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<University> Universities { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //                optionsBuilder.UseSqlServer("Server=INBLRVM26590142; Database=BookStore; Trusted_Connection=true");
        //            }
        //        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AI");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.BookAuthor)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BookName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Course)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BookAllocation>(entity =>
            {
                entity.HasKey(e => e.SerialNo)
                    .HasName("PK__BookAllo__D1FEA253C6383470");

                entity.ToTable("BookAllocation");

                entity.Property(e => e.SerialNo).HasColumnName("serialNo");

                entity.Property(e => e.BookId).HasColumnName("bookId");

                entity.Property(e => e.StudentId).HasColumnName("studentId");

                entity.Property(e => e.UniversityId).HasColumnName("universityId");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookAllocations)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BookAlloc__bookI__32AB8735");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.BookAllocations)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BookAlloc__stude__31B762FC");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.BookAllocations)
                    .HasForeignKey(d => d.UniversityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BookAlloc__unive__339FAB6E");
            });

            //modelBuilder.Entity<Invoice>(entity =>
            //{
            //    entity.ToTable("invoice");

            //    entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");

            //    entity.Property(e => e.Term).HasColumnName("term");

            //    entity.Property(e => e.Tax)
            //        .HasColumnType("decimal(10, 2)")
            //        .HasColumnName("tax");

            //    entity.Property(e => e.TotalAmount)
            //        .HasColumnType("decimal(10, 2)")
            //        .HasColumnName("totalAmount");

            //    entity.Property(e => e.UniversityId).HasColumnName("universityId");

            //    entity.HasOne(d => d.University)
            //        .WithMany(p => p.Invoices)
            //        .HasForeignKey(d => d.UniversityId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK__invoice__univers__40F9A68C");
            //});

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoice");

                entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");

                entity.Property(e => e.BookQuantity).HasColumnName("bookQuantity");

                entity.Property(e => e.Tax)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("tax");

                entity.Property(e => e.Term).HasColumnName("term");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("totalAmount");

                entity.Property(e => e.UniversityId).HasColumnName("universityId");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.UniversityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__invoice__univers__4B7734FF");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.UserRole)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("userRole");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Course)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.University)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.UniversityId)
                    .HasConstraintName("FK__Student__Univers__17036CC0");
            });

            modelBuilder.Entity<University>(entity =>
            {
                entity.ToTable("University");

                entity.HasIndex(e => e.Name, "unique_Name")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "unique_email")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.RoleId).HasDefaultValueSql("((2))");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Id");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.UserSno)
                    .HasName("PK__UserRole__2B6A160F992978A6");

                entity.ToTable("UserRole");

                entity.Property(e => e.UserSno).HasColumnName("UserSNo");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__UserRole__RoleId__6A30C649");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserRole__UserId__693CA210");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
