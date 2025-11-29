namespace MadrasahManagement.Services
{
	public static class ServiceHelper
	{
		public static void AddFileUploader(this IServiceCollection services)
		{
			services.AddScoped<IUploadService, UploadService>();
		}
	}
}
