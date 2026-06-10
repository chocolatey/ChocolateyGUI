using AutoMapper;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.nuget;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.ViewModels;
using NuGet.Protocol.Core.Types;
using ChocolateySource = chocolatey.infrastructure.app.configuration.ChocolateySource;
using Environment = System.Environment;

namespace ChocolateyGui.Common.Windows.Startup
{
    public static class ChocolateyGuiMapper
    {
        public static MapperConfiguration CreateConfiguration()
        {
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.ForAllMaps((_, mapping) => mapping.MaxDepth(64));

                config.CreateMap<IPackageViewModel, IPackageViewModel>()
                    .ForMember(vm => vm.IsInstalled, options => options.Ignore());

                config.CreateMap<IPackageSearchMetadata, Package>()
                    .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Identity.Version))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Identity.Id))
                    .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Split(',')))
                    .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners.Split(',')))
                    .ForMember(dest => dest.GalleryDetailsUrl, opt => opt.MapFrom(src => src.PackageDetailsUrl == null ? null : src.PackageDetailsUrl.AbsoluteUri));

                config.CreateMap<ConfigFileFeatureSetting, ChocolateyFeature>();
                config.CreateMap<ConfigFileConfigSetting, ChocolateySetting>();
                config.CreateMap<ConfigFileSourceSetting, Common.Models.ChocolateySource>()
                    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => NugetEncryptionUtility.DecryptString(src.Password)))
                    .ForMember(dest => dest.CertificatePassword, opt => opt.MapFrom(src => NugetEncryptionUtility.DecryptString(src.CertificatePassword)));

                config.CreateMap<ChocolateySource, Common.Models.ChocolateySource>()
                    .ForMember(dest => dest.VisibleToAdminsOnly, opt => opt.MapFrom(src => src.VisibleToAdminOnly));

                config.CreateMap<AdvancedInstallViewModel, AdvancedInstall>()
                    .ForMember(
                        dest => dest.DownloadChecksum,
                        opt => opt.Condition(source => !source.IgnoreChecksums))
                    .ForMember(
                        dest => dest.DownloadChecksumType,
                        opt => opt.Condition(source =>
                            !source.IgnoreChecksums && !string.IsNullOrEmpty(source.DownloadChecksum)))
                    .ForMember(
                        dest => dest.DownloadChecksum64bit,
                        opt => opt.Condition(source =>
                            Environment.Is64BitOperatingSystem
                            && !source.IgnoreChecksums
                            && !source.Forcex86))
                    .ForMember(
                        dest => dest.DownloadChecksumType64bit,
                        opt => opt.Condition(source =>
                            Environment.Is64BitOperatingSystem
                            && !source.IgnoreChecksums
                            && !source.Forcex86
                            && !string.IsNullOrEmpty(source.DownloadChecksum64bit)))
                    .ForMember(
                        dest => dest.PackageParameters,
                        opt => opt.Condition(source => !source.SkipPowerShell))
                    .ForMember(
                        dest => dest.InstallArguments,
                        opt => opt.Condition(source => !source.SkipPowerShell && !source.NotSilent));
            });

            return mapperConfiguration;
        }
    }
}
