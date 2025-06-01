using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class WebChatContext : DbContext
    {
        public WebChatContext(DbContextOptions<WebChatContext> options) : base(options)
        {
        }
        public WebChatContext () { }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyConnection"));

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<FriendShip>(entity =>
            {
                entity.HasKey(fs => new { fs.UserId, fs.FriendId });

                entity.Property(fs => fs.UpdateAt)
                      .ValueGeneratedOnAddOrUpdate()
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(fs => fs.User)
                      .WithMany(u => u.SenderFriendShips)
                      .HasForeignKey(fs => fs.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fs => fs.Friend)
                      .WithMany(u => u.ReceiverFriendShips)
                      .HasForeignKey(fs => fs.FriendId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(gm => new { gm.GroupId, gm.UserId });

                entity.Property(gm => gm.JoinedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(gm => gm.Group)
                      .WithMany(g => g.GroupMembers)
                      .HasForeignKey(gm => gm.GroupId);

                entity.HasOne(gm => gm.User)
                      .WithMany(u => u.GroupMembersShips)
                      .HasForeignKey(gm => gm.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<MessageStatus>(entity =>
            {
                entity.HasKey(ms => new { ms.MessageId, ms.UserId });
                entity.Property(ms => ms.UpdatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(ms => ms.Message)
                      .WithMany(m => m.MessageStatuses)
                      .HasForeignKey(ms => ms.MessageId);
                entity.HasOne(ms => ms.User)
                      .WithMany(u => u.MessageStatus)
                      .HasForeignKey(ms => ms.UserId);
            });

            modelBuilder.Entity<Message>(entity => {
                entity.Property(m => m.SentAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(m => m.Sender)
                      .WithMany(u => u.SenderMessages)
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Receiver)
                      .WithMany(u => u.ReceiverMessages)
                      .HasForeignKey(m => m.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Group)
                      .WithMany(g => g.Messages)
                      .HasForeignKey(m => m.GroupId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(g => g.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });
        }

    }
}
