using System;
using System.Text;
using Verona.Lib.o365.App.Interface;

namespace Verona.Lib.o365.App.Object
{
    public class TaxonomyFieldSchema : ITaxonomyFieldSchema
    {
        /// <summary>
        /// Gets or sets the term store unique identifier.
        /// </summary>
        /// <value>
        /// The term store unique identifier.
        /// </value>
        public string TermStoreGuid { get; set; }
        /// <summary>
        /// Gets or sets the term set unique identifier.
        /// </summary>
        /// <value>
        /// The term set unique identifier.
        /// </value>
        public string TermSetGuid { get; set; }
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whetherthe field is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [required field]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiredField { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the field must have unique value(s).
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enforce unique values]; otherwise, <c>false</c>.
        /// </value>
        public bool EnforceUniqueValues { get; set; }
        /// <summary>
        /// Gets or sets the lcid.
        /// </summary>
        /// <value>
        /// The lcid.
        /// </value>
        public int Lcid { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the user should be able to add new values to the termset.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [open]; otherwise, <c>false</c>.
        /// </value>
        public bool Open { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the field should be able to hold multiple terms.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multi]; otherwise, <c>false</c>.
        /// </value>
        public bool Multi { get; set; }
        /// <summary>
        /// Gets or sets the note field unique identifier.
        /// </summary>
        /// <value>
        /// The note field unique identifier.
        /// </value>
        private string NoteFieldGuid { get; set; }

        /// <summary>
        /// Gets the note field schema.
        /// </summary>
        /// <returns></returns>
        public string GetNoteFieldSchema()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return string.Empty;

            var noteSchema = new StringBuilder();
            NoteFieldGuid = Guid.NewGuid().ToString();
            var nameGuid = Guid.NewGuid();

            noteSchema.AppendFormat("<Field DisplayName=\"{0}_0\" Type=\"Note\" ", DisplayName);
            noteSchema.AppendFormat("StaticName=\"{0}\" ", nameGuid);
            noteSchema.AppendFormat("Name=\"{0}\" ", nameGuid);
            noteSchema.Append("ID=\"{" + NoteFieldGuid + "}\" ");
            noteSchema.Append("ShowInViewForms=\"FALSE\" ");
            noteSchema.Append("Required=\"FALSE\" ");
            noteSchema.Append("Hidden=\"TRUE\" ");
            noteSchema.Append("CanToggleHidden=\"TRUE\" ");
            noteSchema.Append(" />");

            return noteSchema.ToString();
        }

        /// <summary>
        /// Gets the multi value taxonomy field schema.
        /// </summary>
        /// <returns></returns>
        public string GetTaxonomyFieldSchemaMulti()
        {
            if (string.IsNullOrEmpty(NoteFieldGuid) || string.IsNullOrEmpty(TermStoreGuid) || string.IsNullOrEmpty(TermSetGuid))
                return string.Empty;

            var taxSchema = new StringBuilder();

            taxSchema.Append("<Field Type=\"TaxonomyFieldTypeMulti\" ");
            taxSchema.AppendFormat("DisplayName=\"{0}\" ", DisplayName);
            taxSchema.Append("Mult=\"TRUE\" ");
            taxSchema.AppendFormat("ShowField=\"Term{0}\" ", Lcid);
            taxSchema.AppendFormat("Required=\"{0}\" ", RequiredField ? "TRUE" : "FALSE");
            taxSchema.AppendFormat("EnforceUniqueValues=\"{0}\" ", EnforceUniqueValues ? "TRUE" : "FALSE");
            taxSchema.Append("ID=\"{" + Guid.NewGuid() + "}\" ");
            taxSchema.AppendFormat("StaticName=\"{0}\" ", Name);
            taxSchema.AppendFormat("Name=\"{0}\" ", Name);
            taxSchema.Append(">");
            taxSchema.Append("<Default></Default>");
            taxSchema.Append("<Customization>");
            taxSchema.Append("<ArrayOfProperty>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>SspId</Name>");
            taxSchema.AppendFormat("<Value xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q1:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{0}</Value>", TermStoreGuid);
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>GroupId</Name>");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>TermSetId</Name>");
            taxSchema.AppendFormat("<Value xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q2:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{0}</Value>", TermSetGuid);
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>AnchorId</Name>");
            taxSchema.Append("<Value xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q3:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">00000000-0000-0000-0000-000000000000</Value>");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>UserCreated</Name>");
            taxSchema.Append("<Value xmlns:q4=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q4:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value>");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>Open</Name>");
            taxSchema.AppendFormat("<Value xmlns:q5=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q5:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{0}</Value>", Open ? "true" : "false");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>TextField</Name>");
            taxSchema.Append("<Value xmlns:q6=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q6:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{" + NoteFieldGuid + "}</Value>");
            taxSchema.Append("</Property>");
            taxSchema.Append("</ArrayOfProperty>");

            taxSchema.Append("</Customization>");
            taxSchema.Append("</Field>");

            return taxSchema.ToString();
        }

        /// <summary>
        /// Gets the single value taxonomy field schema.
        /// </summary>
        /// <returns></returns>
        public string GetTaxonomyFieldSchema()
        {
            if (string.IsNullOrEmpty(NoteFieldGuid) || string.IsNullOrEmpty(TermStoreGuid) || string.IsNullOrEmpty(TermSetGuid))
                return string.Empty;

            var taxSchema = new StringBuilder();

            taxSchema.Append("<Field Type=\"TaxonomyFieldType\" ");
            taxSchema.AppendFormat("DisplayName=\"{0}\" ", DisplayName);
            taxSchema.AppendFormat("ShowField=\"Term{0}\" ", Lcid);
            taxSchema.AppendFormat("Required=\"{0}\" ", RequiredField ? "TRUE" : "FALSE");
            taxSchema.AppendFormat("EnforceUniqueValues=\"{0}\" ", EnforceUniqueValues ? "TRUE" : "FALSE");
            taxSchema.Append("ID=\"{" + Guid.NewGuid() + "}\" ");
            taxSchema.AppendFormat("StaticName=\"{0}\" ", Name);
            taxSchema.AppendFormat("Name=\"{0}\" ", Name);
            taxSchema.Append(">");
            taxSchema.Append("<Default></Default>");
            taxSchema.Append("<Customization>");
            taxSchema.Append("<ArrayOfProperty>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>SspId</Name>");
            taxSchema.AppendFormat("<Value xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q1:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{0}</Value>", TermStoreGuid);
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>GroupId</Name>");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>TermSetId</Name>");
            taxSchema.AppendFormat("<Value xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q2:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{0}</Value>", TermSetGuid);
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>AnchorId</Name>");
            taxSchema.Append("<Value xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q3:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">00000000-0000-0000-0000-000000000000</Value>");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>UserCreated</Name>");
            taxSchema.Append("<Value xmlns:q4=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q4:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value>");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>Open</Name>");
            taxSchema.AppendFormat("<Value xmlns:q5=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q5:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{0}</Value>", Open ? "true" : "false");
            taxSchema.Append("</Property>");

            taxSchema.Append("<Property>");
            taxSchema.Append("<Name>TextField</Name>");
            taxSchema.Append("<Value xmlns:q6=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q6:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{" + NoteFieldGuid + "}</Value>");
            taxSchema.Append("</Property>");
            taxSchema.Append("</ArrayOfProperty>");

            taxSchema.Append("</Customization>");
            taxSchema.Append("</Field>");

            return taxSchema.ToString();
        }
    }
}
