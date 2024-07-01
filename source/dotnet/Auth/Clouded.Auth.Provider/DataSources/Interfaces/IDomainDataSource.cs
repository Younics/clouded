using Clouded.Auth.Provider.Dictionaries;

namespace Clouded.Auth.Provider.DataSources.Interfaces;

public interface IDomainDataSource : IDataSource<DomainDictionary>
{
    public IEnumerable<MachineDictionary> DomainMachines(object domainId);

    public void DomainExtendWithMachines(DomainDictionary domain);
    public void DomainMachinesAdd(object id, IEnumerable<object> machineIds);
    public void DomainMachinesRemove(object id, IEnumerable<object> machineIds);
}
