namespace Sitecore.Support.ContentSearch.Azure
{
  using System.Collections.Concurrent;

  using Sitecore.Abstractions;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Azure;
  using Sitecore.ContentSearch.Diagnostics;
  using Sitecore.ContentSearch.Linq.Common;
  using Sitecore.ContentSearch.Pipelines.IndexingFilters;
  using Sitecore.Diagnostics;
  using Sitecore.Reflection;
  public class CloudSearchIndexOperations : IIndexOperations
  {

    private readonly ISearchIndex index;

    public CloudSearchIndexOperations(ISearchIndex index)
    {
      Assert.ArgumentNotNull(index, "index");
      this.index = index;
    }

    protected virtual ConcurrentDictionary<string, object> GetDocument(IIndexable indexable, IProviderUpdateContext context)
    {
      var instance = context.Index.Locator.GetInstance<ICorePipeline>();
      //indexable = CleanUpPipeline.Run(instance, new CleanUpArgs(indexable, context));

      if (InboundIndexFilterPipeline.Run(instance, new InboundIndexFilterArgs(indexable)))
      {
        this.index.Locator.GetInstance<IEvent>().RaiseEvent("indexing:excludedfromindex", new object[] { this.index.Name, indexable.UniqueId });
        return null;
      }

      var builder = this.GetDocumentBuilderForIndexable(indexable, context);
      builder.BuildDocument();
      return builder.Document;
    }

    public void Update(IIndexable indexable, IProviderUpdateContext context, ProviderIndexConfiguration indexConfiguration)
    {
      var doc = this.GetDocument(indexable, context);
      if (doc != null)
      {
        context.UpdateDocument(doc, null, (IExecutionContext[])null);
      }
    }

    public void Delete(IIndexable indexable, IProviderUpdateContext context)
    {
      context.Delete(indexable.UniqueId);
    }

    public void Delete(IIndexableId id, IProviderUpdateContext context)
    {
      context.Delete(id);
    }

    public void Delete(IIndexableUniqueId id, IProviderUpdateContext context)
    {
      context.Delete(id);
    }

    public void Add(IIndexable indexable, IProviderUpdateContext context, ProviderIndexConfiguration indexConfiguration)
    {
      var doc = this.GetDocument(indexable, context);
      if (doc != null)
      {
        context.AddDocument(doc, (IExecutionContext[])null);
      }
    }
    /// <summary>
    /// Returns a document builder which produces an index document for indexable.
    /// </summary>
    /// <param name="indexable">The indexable</param>
    /// <param name="context">The index update context</param>
    /// <returns></returns>
    protected virtual CloudSearchDocumentBuilder GetDocumentBuilderForIndexable(IIndexable indexable, IProviderUpdateContext context)
    {
      // The implementation has been taken from LuceneIndexOperations and SolrIndexOperations types
      var builder = (CloudSearchDocumentBuilder)ReflectionUtil.CreateObject(context.Index.Configuration.DocumentBuilderType, new object[] { indexable, context });

      if (builder == null)
      {
        CrawlingLog.Log.Error($"[Index={context.Index.Name}] Unable to create document builder (" + context.Index.Configuration.DocumentBuilderType + "). Please check your configuration. We will fallback to the default for now.");
        builder = new CloudSearchDocumentBuilder(indexable, context);
      }

      return builder;
    }
  }
}
