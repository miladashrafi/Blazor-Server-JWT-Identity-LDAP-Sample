namespace Backend.API.Settings;

public class LdapSetting
{
    public string LdapPath { get; set; }
    public string LdapDomain { get; set; }
    public string LdapAdminUser { get; set; }
    public string LdapAdminPassword { get; set; }
    public bool Enable { get; set; }
}