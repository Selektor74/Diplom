using ProjectFishing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectFishing.Infrastructure
{
    public class Context : DbContext
    {
        public Context() : base("DbFishing") { }
        public DbSet<Fish> Fishies { get; set; }
        public DbSet<Lake> Lakes { get; set; }
        public DbSet<Shop> Shops { get; set; }
       
        public DbSet<Post> Posts { get; set; }
        public DbSet<CategoryAd> Categories { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ThemeDiscussion> Thems { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }
        public DbSet<News> News { get; set; }
        // public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().HasKey(p => p.PostId);

            modelBuilder.Entity<Chat>().HasMany(x => x.Messages).WithRequired(x => x.Chat);
            modelBuilder.Entity<Chat>().HasKey(x => x.ChatId);
            modelBuilder.Entity<ThemeDiscussion>().HasKey(t => t.ThemeDiscussionId);
            modelBuilder.Entity<Discussion>().HasKey(d => d.DiscussionId);
            modelBuilder.Entity<ThemeDiscussion>().HasMany(t => t.Discussions).WithRequired(d => d.Theme);
            modelBuilder.Entity<ForumComment>().HasKey(d => d.ForumCommentsId);
            base.OnModelCreating(modelBuilder);
        }

    }

}