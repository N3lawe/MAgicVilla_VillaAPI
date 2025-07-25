﻿using static MagicVilla_Utility.SD;

namespace MAgicVilla_VillaAPI.Models;

public class APIRequest
{
    public ApiType ApiType { get; set; } = ApiType.GET;
    public string Url { get; set; }
    public object Data { get; set; }
    public string Token { get; set; }
}
