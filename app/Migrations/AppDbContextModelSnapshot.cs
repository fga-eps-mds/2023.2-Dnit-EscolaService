﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using app.Entidades;

#nullable disable

namespace app.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("app.Entidades.Escola", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.Property<int>("Codigo")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DataAtualizacaoUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("DistanciaSuperintendencia")
                        .HasColumnType("double precision");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)");

                    b.Property<int?>("Localizacao")
                        .HasColumnType("integer");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)");

                    b.Property<int?>("MunicipioId")
                        .HasColumnType("integer");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Observacao")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("Porte")
                        .HasColumnType("integer");

                    b.Property<int>("Rede")
                        .HasColumnType("integer");

                    b.Property<int?>("Situacao")
                        .HasColumnType("integer");

                    b.Property<int>("SuoerintendenciaId")
                        .HasColumnType("integer");

                    b.Property<int>("SuperintendenciaId")
                        .HasColumnType("integer");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<int>("TotalAlunos")
                        .HasColumnType("integer");

                    b.Property<int>("TotalDocentes")
                        .HasColumnType("integer");

                    b.Property<int?>("Uf")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MunicipioId");

                    b.HasIndex("SuperintendenciaId");

                    b.ToTable("Escolas");
                });

            modelBuilder.Entity("app.Entidades.EscolaEtapaEnsino", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EscolaId")
                        .HasColumnType("uuid");

                    b.Property<int>("EtapaEnsino")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EscolaId");

                    b.ToTable("EscolaEtapaEnsino");
                });

            modelBuilder.Entity("app.Entidades.Municipio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Uf")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Municipios");
                });

            modelBuilder.Entity("app.Entidades.Superintendencia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Uf")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Superintendencias");
                });

            modelBuilder.Entity("app.Entidades.Escola", b =>
                {
                    b.HasOne("app.Entidades.Municipio", "Municipio")
                        .WithMany()
                        .HasForeignKey("MunicipioId");

                    b.HasOne("app.Entidades.Superintendencia", "Superintendencia")
                        .WithMany()
                        .HasForeignKey("SuperintendenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Municipio");

                    b.Navigation("Superintendencia");
                });

            modelBuilder.Entity("app.Entidades.EscolaEtapaEnsino", b =>
                {
                    b.HasOne("app.Entidades.Escola", "Escola")
                        .WithMany("EtapasEnsino")
                        .HasForeignKey("EscolaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Escola");
                });

            modelBuilder.Entity("app.Entidades.Escola", b =>
                {
                    b.Navigation("EtapasEnsino");
                });
#pragma warning restore 612, 618
        }
    }
}
