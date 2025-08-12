namespace minimal_api.Domain.ModelViews
{
    public struct Home
    {
        public readonly string Message { get => "Wellcome to the Minimal API Example"; }
        public readonly string Documentation { get => "/docs"; }
    }
}