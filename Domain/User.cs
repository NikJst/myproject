namespace Testing3;
public class User
{

    public Guid GuidId { get; set; }
    public string Name { get; set; }
    public List<Post> Posts { get; private set; } = new(); // Связь "Один ко многим"

    // Конструктор для создания пользователя с именем
    public User(string name)
    {
        Name = name ?? throw new ArgumentNullException("Name required");
    }

}
public class Guest : User
{
    public Guest() : base("") { } // пустой конструктор с базовым именем
}