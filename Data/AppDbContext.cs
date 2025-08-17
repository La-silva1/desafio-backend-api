using Microsoft.EntityFrameworkCore;
using DesafioBackend.Motos;
using DesafioBackend.Entregadores;
using DesafioBackend.Locacoes;
using System;

namespace DesafioBackend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Moto> Motos { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

    }
}


