namespace Verona.Lib.o365.App.Interface
{
    public interface ITaxonomyFieldSchema
    {
        string TermStoreGuid { get; set; }
        string TermSetGuid { get; set; }
        string DisplayName { get; set; }
        string Name { get; set; }
        bool RequiredField { get; set; }
        bool EnforceUniqueValues { get; set; }
        int Lcid { get; set; }
        bool Open { get; set; }
        bool Multi { get; set; }

        string GetNoteFieldSchema();
        string GetTaxonomyFieldSchemaMulti();
        string GetTaxonomyFieldSchema();
    }
}
