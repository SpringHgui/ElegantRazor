namespace ConsoleSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            using (var render = new ElegantRazor.ElegantRazor())
            {
                // render.Initialize(typeof(RazorTemplate.Pages.IndexModel).Assembly);
                render.Initialize(null, "MVCTemplate");
                var viewData = new ElegantRazor.DynamicViewData();
                viewData.AddValue("Title", "测试");

                var html = render.RenderViewAsync<object>("Views/Home/Index.cshtml", null, viewData).GetAwaiter().GetResult();

                Console.WriteLine(html);
            }

            Console.ReadLine();
        }
    }
}