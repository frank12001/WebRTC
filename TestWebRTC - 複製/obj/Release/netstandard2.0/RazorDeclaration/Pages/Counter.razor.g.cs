#pragma checksum "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\Pages\Counter.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2cb13ecdab24bf3ea8479b8c2d07c9e70a17817e"
// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace TestWebRTC.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#line 1 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#line 2 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#line 3 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using Microsoft.AspNetCore.Components.Layouts;

#line default
#line hidden
#line 4 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#line 5 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#line 6 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using TestWebRTC;

#line default
#line hidden
#line 7 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\_Imports.razor"
using TestWebRTC.Shared;

#line default
#line hidden
    [Microsoft.AspNetCore.Components.Layouts.LayoutAttribute(typeof(MainLayout))]
    [Microsoft.AspNetCore.Components.RouteAttribute("/counter")]
    public class Counter : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder)
        {
        }
        #pragma warning restore 1998
#line 9 "C:\Users\User\Desktop\BlazorTest\TestWebRTC - 複製\Pages\Counter.razor"
       
    int currentCount = 0;

    void IncrementCount()
    {
        currentCount++;
    }

#line default
#line hidden
    }
}
#pragma warning restore 1591
