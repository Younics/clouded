using Clouded.Admin.Provider.DataSources.Dictionaries;

namespace Clouded.Admin.Provider.DataSources.Interfaces;

public interface IUserSettingsDataSource
{
    void TableVerification();
    void SupportTableSetup();
    public void Update(string userId, UserSettingsDictionary data);
    public void Create(UserSettingsDictionary data);
    public UserSettingsDictionary? GetByUserId(string userId);
}
