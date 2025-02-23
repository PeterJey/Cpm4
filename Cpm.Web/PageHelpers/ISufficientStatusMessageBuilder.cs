namespace Cpm.Web.PageHelpers
{
    public interface ISufficientStatusMessageBuilder : IEmptyStatusMessageBuilder
    {
        ISufficientStatusMessageBuilder Strong(string text, params object[] args);
        void Dismissible();
        void Static();
    }
}