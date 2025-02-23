namespace Cpm.Web.PageHelpers
{
    public interface IBlankStatusMessageBuilder
    {
        IEmptyStatusMessageBuilder Success();
        IEmptyStatusMessageBuilder Info();
        IEmptyStatusMessageBuilder Warning();
        IEmptyStatusMessageBuilder Danger();
    }
}