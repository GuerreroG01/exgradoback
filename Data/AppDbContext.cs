using Microsoft.EntityFrameworkCore;
using ExGradoBack.Models;

namespace ExGradoBack.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //Modelos para el Manejo de usuarios en el sistema.
        public DbSet<Auth> Auth { get; set; }
        public DbSet<InfoUser> InfoUsers { get; set; }
        public DbSet<Rol> Rol { get; set; }
        //Modelo para Respaldo
        public DbSet<Backup> Backup { get; set; }
        public DbSet<ActividadFactura> ActividadFactura { get; set; }

        //Modelos para el Negocio
        public DbSet<VehiculoInfo> VehiculoInfo { get; set; }
        public DbSet<MarcaRepuesto> MarcaRepuesto { get; set; }
        public DbSet<Repuesto> Repuesto { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }

        public DbSet<Factura> Factura { get; set; }
        public DbSet<DetalleFactura> DetalleFactura { get; set; }

        public DbSet<OrdenCompra> OrdenCompra { get; set; }
        public DbSet<DetalleOrdenCompra> DetalleOrdenCompra { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auth>()
                .HasOne(a => a.InfoUser)
                .WithOne(i => i.Auth)
                .HasForeignKey<InfoUser>(i => i.AuthId);
            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Repuesto>()
                .HasMany(r => r.VehiculoInfoIds)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "RepuestoVehiculoInfo",
                    r => r.HasOne<VehiculoInfo>().WithMany().HasForeignKey("VehiculoInfoId"),
                    v => v.HasOne<Repuesto>().WithMany().HasForeignKey("RepuestoId")
                );

            modelBuilder.Entity<Repuesto>()
                .HasOne(r => r.MarcaRepuesto)
                .WithMany()
                .HasForeignKey(r => r.MarcaRepuestoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Factura)
                .WithMany(f => f.Detalles)
                .HasForeignKey(df => df.FacturaId);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Repuesto)
                .WithMany()
                .HasForeignKey(df => df.RepuestoId);

            modelBuilder.Entity<OrdenCompra>()
                .HasOne(oc => oc.Proveedor)
                .WithMany()
                .HasForeignKey(oc => oc.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleOrdenCompra>()
                .HasOne(doc => doc.OrdenCompra)
                .WithMany(oc => oc.Detalles)
                .HasForeignKey(doc => doc.OrdenCompraId);

            modelBuilder.Entity<DetalleOrdenCompra>()
                .HasOne(doc => doc.Repuesto)
                .WithMany()
                .HasForeignKey(doc => doc.RepuestoId);

            //Datos Predeterminados
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Invitado" },
                new Rol { Id = 3, Nombre = "Gerente" },
                new Rol { Id = 4, Nombre = "Supervisor" },
                new Rol { Id = 5, Nombre = "Cajero" },
                new Rol { Id = 6, Nombre = "Inventario" }
            );
            modelBuilder.Entity<Auth>().HasData(
            new Auth
            {
                Id = 1,
                Username = "admin",
                Password = "$2a$11$kvmh2pY5/uqViNOj5A9OTOdqi9cRjSRFsbdmKfzEpkLcXTceTe8rS",
                FechaRegistro = DateTime.Now,
                RolId = 1
            },
            new Auth
            {
                Id = 2,
                Username = "Invitado",
                Password = "$2a$11$kvmh2pY5/uqViNOj5A9OTOdqi9cRjSRFsbdmKfzEpkLcXTceTe8rS",
                FechaRegistro = DateTime.Now,
                RolId = 2
            }
        );
            //Indices
            modelBuilder.Entity<Auth>()
                .HasIndex(a => a.Username)
                .HasDatabaseName("IX_Auth_Username");

            modelBuilder.Entity<VehiculoInfo>()
                .HasIndex(v => new { v.Marca, v.Anio })
                .HasDatabaseName("IX_VehiculoInfo_Marca_Anio");

            modelBuilder.Entity<MarcaRepuesto>()
                .HasIndex(m => m.Nombre)
                .HasDatabaseName("IX_Nombre_Marca");
            modelBuilder.Entity<MarcaRepuesto>()
                .HasIndex(m => m.Calificacion)
                .HasDatabaseName("IX_Calificacion_Marca");

            modelBuilder.Entity<Repuesto>()
                .HasIndex(r => r.Nombre)
                .HasDatabaseName("IX_Nombre_Repuesto");
            modelBuilder.Entity<Repuesto>()
                .HasIndex(r => r.Ubicacion)
                .HasDatabaseName("IX_Ubicacion_Repuesto");
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.Nombre)
                .HasDatabaseName("IX_Nombre_Proveedor");
            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.Fecha)
                .HasDatabaseName("IX_Factura_Fecha");
            modelBuilder.Entity<OrdenCompra>()
                .HasIndex(o => o.Fecha)
                .HasDatabaseName("IX_OrdenCompra_Fecha");
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.Nombre)
                .HasDatabaseName("IX_Nombre_Proveedor");
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => new { p.Pais, p.Ciudad })
                .HasDatabaseName("IX_PaisCiudad_Proveedor");


            base.OnModelCreating(modelBuilder);
        }
    }
}