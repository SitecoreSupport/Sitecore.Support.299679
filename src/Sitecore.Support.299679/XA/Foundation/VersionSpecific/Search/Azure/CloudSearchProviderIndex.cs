namespace Sitecore.Support.XA.Foundation.VersionSpecific.Search.Azure
{
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Azure;
  using Sitecore.ContentSearch.Azure.Query;
  using Sitecore.ContentSearch.Maintenance;
  using Sitecore.ContentSearch.Security;
  using Sitecore.XA.Foundation.VersionSpecific.Search.Azure;

  public class CloudSearchProviderIndex : Sitecore.Support.ContentSearch.Azure.CloudSearchProviderIndex
  {
    public CloudSearchProviderIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore)
        : base(name, connectionStringName, totalParallelServices, propertyStore)
    {
    }

    public override IProviderSearchContext CreateSearchContext(SearchSecurityOptions options = SearchSecurityOptions.EnableSecurityCheck)
    {
      base.CreateSearchContext(options);
      return new Sitecore.XA.Foundation.VersionSpecific.Search.Azure.CloudSearchSearchContext(InitializeServiceCollectionClient(), options);
    }

    protected virtual ServiceCollectionClient InitializeServiceCollectionClient()
    {
      ServiceCollectionClient serviceCollectionClient = new ServiceCollectionClient();
      serviceCollectionClient.Register(typeof(Sitecore.ContentSearch.Azure.Query.LinqToCloudIndex<>), (ServiceBuilder builder) => builder.CreateInstance(typeof(Sitecore.XA.Foundation.VersionSpecific.Search.Azure.LinqToCloudIndex<>), builder.TypeParameters, builder.Args));
      serviceCollectionClient.Register(typeof(AbstractSearchIndex), (ServiceBuilder builder) => this);
      serviceCollectionClient.Register(typeof(QueryStringBuilder), (ServiceBuilder builder) => new QueryStringBuilder(new FilterQueryBuilder(), new SearchQueryBuilder(), true));
      return serviceCollectionClient;
    }
  }
}