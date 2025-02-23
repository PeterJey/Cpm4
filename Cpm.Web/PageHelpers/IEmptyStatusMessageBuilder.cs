namespace Cpm.Web.PageHelpers
{
    public interface IEmptyStatusMessageBuilder
    {
        ISufficientStatusMessageBuilder Text(string text, params object[] args);
        ISufficientStatusMessageBuilder Link(string text, string url);
    }
}