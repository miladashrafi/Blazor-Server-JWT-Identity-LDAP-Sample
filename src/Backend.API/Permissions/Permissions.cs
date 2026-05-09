using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Backend.API.Permissions;

public class Permission
{
    protected Permission()
    {
    }

    protected Permission(string key, string title, string value = "")
    {
        Nodes = new List<Permission>();
        Key = key;
        Title = title;
        Value = string.IsNullOrEmpty(value) ? key : value;
        IsChecked = false;
    }

    public string Value { get; }
    public string Title { get; private set; }

    [JsonIgnore] public string Key { get; }

    public bool IsChecked { get; set; }
    public ICollection<Permission> Nodes { get; }

    public Permission AddNode(string key, string title)
    {
        var item = new Permission(key, title, $"{Value}.{key}");
        Nodes.Add(item);
        return item;
    }
}

public class PermissionList
{
    private readonly List<Permission> _permissions;

    public PermissionList()
    {
        _permissions = new List<Permission>();
        _permissions.Add(new AdministrativePermission());
    }

    public ICollection<Permission> Permissions => new Collection<Permission>(_permissions);

    public Permission GetPermissionByKey(string key)
    {
        var sections = key.Split('.');
        var keys = new Stack<string>(sections.Reverse());
        return FindPermission(keys, _permissions);
    }

    private Permission FindPermission(Stack<string> keys, ICollection<Permission> permissions)
    {
        var st = keys.Pop();
        var t = permissions.FirstOrDefault(x => x.Key == st);
        if (t == null) return null;
        if (t.Nodes.Count == 0) return t;
        return FindPermission(keys, t.Nodes);
    }

    public void SetPermissionEnable(string key)
    {
        var sections = key.Split('.');
        var keys = new Stack<string>(sections.Reverse());
        var item = FindPermission(keys, _permissions);
        if (item != null) item.IsChecked = true;
    }
}

public class AdministrativePermission : Permission
{
    public AdministrativePermission() : base("Administrative", "Manage Users")
    {
        AddNode("ManageUsers", "Manage Users");
        AddNode("ViewUsers", "View Users");
    }

    public static string AdministrativeManageUser => "Administrative.ManageUsers";
    public static string AdministrativeViewUser => "Administrative.ViewUsers";
}
