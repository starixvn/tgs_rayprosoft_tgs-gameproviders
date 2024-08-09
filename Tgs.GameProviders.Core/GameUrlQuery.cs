using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tgs.GameProviders.Core;

public record GameUrlQuery
{
	[Required(ErrorMessage = "Please enter PlayerCode")]
	[DefaultValue(typeof(string), "4ucmqplxb029")]
	public string PlayerCode { get; init; } = default!;

	[Required(ErrorMessage = "Please enter GameId")]
	[DefaultValue(typeof(int), "1")]
	public int GameId { get; init; } = default!;

	[Required(ErrorMessage = "Please enter LoginRecordId")]
	[DefaultValue(typeof(string), "00000000-0000-0000-0000-000000000000")]
	public string LoginRecordId { get; init; } = default!;

	[Required(ErrorMessage = "Please enter PlatformType")]
	[DefaultValue(typeof(PlatformType), "Desktop")]
	public PlatformType PlatformType { get; init; }

	[Required(ErrorMessage = "Please enter Language")]
	[DefaultValue(typeof(string), "en-US")]
	public string Language { get; init; } = default!;

	[Required(ErrorMessage = "Please enter IPAddress")]
	[DefaultValue(typeof(string), "127.0.0.1")]
	public string IPAddress { get; init; } = default!;

	[DefaultValue(typeof(Uri), "https://dev-t1.rpgp.cc/")]
	public Uri BackUrl { get; init; }
}
