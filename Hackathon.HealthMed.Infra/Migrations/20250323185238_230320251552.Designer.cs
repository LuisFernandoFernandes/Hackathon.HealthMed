﻿// <auto-generated />
using System;
using Hackathon.HealthMed.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Hackathon.HealthMed.Infra.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20250323185238_230320251552")]
    partial class _230320251552
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Agendamento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("smalldatetime");

                    b.Property<Guid>("HorarioId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("JustificativaCancelamento")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("PacienteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HorarioId");

                    b.HasIndex("PacienteId");

                    b.ToTable("Agendamentos", (string)null);
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Horario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("DataHorario")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("MedicoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("Id");

                    b.HasIndex("MedicoId", "DataHorario")
                        .IsUnique()
                        .HasDatabaseName("IX_Horarios_Medico_DataHorario");

                    b.ToTable("Horarios", (string)null);
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Medico", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CRM")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("smalldatetime");

                    b.Property<int>("Especialidade")
                        .HasColumnType("int");

                    b.Property<Guid>("UsuarioId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId")
                        .IsUnique();

                    b.ToTable("Medicos", (string)null);
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Paciente", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("smalldatetime");

                    b.Property<Guid>("UsuarioId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId")
                        .IsUnique();

                    b.ToTable("Pacientes", (string)null);
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Usuario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Ativo")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("Email")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SenhaHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("TipoUsuario")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Usuarios", (string)null);
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Agendamento", b =>
                {
                    b.HasOne("Hackathon.HealthMed.Domain.Entities.Horario", "Horario")
                        .WithMany()
                        .HasForeignKey("HorarioId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Hackathon.HealthMed.Domain.Entities.Paciente", "Paciente")
                        .WithMany()
                        .HasForeignKey("PacienteId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Horario");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Horario", b =>
                {
                    b.HasOne("Hackathon.HealthMed.Domain.Entities.Medico", "Medico")
                        .WithMany()
                        .HasForeignKey("MedicoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medico");
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Medico", b =>
                {
                    b.HasOne("Hackathon.HealthMed.Domain.Entities.Usuario", "Usuario")
                        .WithOne()
                        .HasForeignKey("Hackathon.HealthMed.Domain.Entities.Medico", "UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Hackathon.HealthMed.Domain.Entities.Paciente", b =>
                {
                    b.HasOne("Hackathon.HealthMed.Domain.Entities.Usuario", "Usuario")
                        .WithOne()
                        .HasForeignKey("Hackathon.HealthMed.Domain.Entities.Paciente", "UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });
#pragma warning restore 612, 618
        }
    }
}
