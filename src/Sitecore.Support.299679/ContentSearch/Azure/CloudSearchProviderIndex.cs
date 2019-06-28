namespace Sitecore.Support.ContentSearch.Azure
{
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Azure;
  using Sitecore.ContentSearch.Maintenance;

  /// <summary>
  /// The indexer which will create and populate search index to Azure Cloud Search Service
  /// </summary>
  public class CloudSearchProviderIndex : Sitecore.ContentSearch.Azure.CloudSearchProviderIndex
  {
    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore)
      : base(name, connectionStringName, totalParallelServices, propertyStore)
    {
    }

    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore, ICloudSearchProviderIndexName cloudSearchProviderIndexName) : base(name, connectionStringName, totalParallelServices, propertyStore, cloudSearchProviderIndexName)
    {
    }

    public override IIndexOperations Operations => new Sitecore.Support.ContentSearch.Azure.CloudSearchIndexOperations(this);

  }
}