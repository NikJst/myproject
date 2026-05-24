using Microsoft.EntityFrameworkCore;
namespace Testing3;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) //цепочка
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация для DateTime - используем UTC
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        //	•	HasOne — это с точки зрения текущей сущности.
        //	•	WithMany / WithOne — это с точки зрения той сущности, к которой вы привязываетесь.

        modelBuilder.Entity<Like>() // все ниже описанное действительно к одной (1) сущности LIKE
                    .HasOne(l => l.User)             // У одной LIKE есть один автор (User) LIKE не могут делить оба автора. 
                    .WithMany(u => u.Likes)         // субьективная точка зрения POST на сущность LIKE - один POST может иметь много сущностей LIKE
                    .HasForeignKey(l => l.UserId) // В таблице Like поле UserId является ключом
                    .OnDelete(DeleteBehavior.Cascade); // Если удалить пользователя, его лайки удалятся автоматически

        modelBuilder.Entity<Like>()
                    .HasOne(l => l.Post)
                    .WithMany(p => p.Likes) // У одного поста может быть много лайков
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bookmark>() // все ниже описанное действительно к одной (1) сущности LIKE
                           .HasOne(b => b.User)             // У одной LIKE есть один автор (User) LIKE не могут делить оба автора. 
                           .WithMany(u => u.Bookmarks)         // субьективная точка зрения POST на сущность LIKE - один POST может иметь много сущностей LIKE
                           .HasForeignKey(b => b.UserId) // В таблице Like поле UserId является ключом
                           .OnDelete(DeleteBehavior.Cascade); // Если удалить пользователя, его лайки удалятся автоматически
        modelBuilder.Entity<Bookmark>()
                    .HasOne(b => b.Post)
                    .WithMany(p => p.Bookmarks) // У одного поста может быть много лайков
                    .HasForeignKey(b => b.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bookmark>()
            .HasKey(b => new { b.UserId, b.PostId });

        modelBuilder.Entity<Post>()
        .HasOne(p => p.User)
        .WithMany(u => u.Posts)
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
        .HasMany(u => u.Posts)
        .WithOne(p => p.User)               //==> пост принадлежит одному пользователю
        .OnDelete(DeleteBehavior.Cascade);

        // 3. Композитный ключ (ВАЖНО!)
        // Чтобы один пользователь не мог лайкнуть один и тот же пост дважды
        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.UserId, l.PostId });

    }
}
