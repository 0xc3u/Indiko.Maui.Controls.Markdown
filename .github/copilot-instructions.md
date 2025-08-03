This is a c# (.net9) based repository with a different projects (Maui.net Library, Maui.net App) representing a markdown viewer for maui.net. 


Please follow these guidelines when contributing:

## Code Standards

### General Principles
- **Target Framework:** All projects target .NET 9.0. Ensure compatibility with this version.
- **Language:** Use C# for all code unless otherwise specified.
- **Readability:** Write clear, self-explanatory code. Use meaningful variable, method, and class names.
- **Consistency:** Follow consistent naming conventions and code formatting throughout the solution.
- **Comments:** Add inline comments only where necessary to clarify complex logic.
- **Error Handling:** Use structured exception handling. Avoid swallowing exceptions; log or rethrow as appropriate.
- **SOLID Principles:** Adhere to SOLID design principles for maintainable and extensible code.
- **Dependency Injection:** Prefer constructor injection for dependencies.
- **Async/Await:** Use asynchronous programming patterns where appropriate, especially for I/O-bound operations.
- **Magic Numbers:** Avoid magic numbers; use named constants or enums.
- **Follow existing Code principles:** Search for similar implementation of other domain parts and adopt the style as blueprint.

- ### Naming Conventions
- **Classes & Interfaces:** PascalCase (e.g., `UserService`, `IUserRepository`)
- **Methods & Properties:** PascalCase (e.g., `GetUserById`)
- **Variables & Parameters:** camelCase (e.g., `userId`)
- **Constants:** PascalCase (e.g., `DefaultTimeout`)
- **Unit Test Methods:** Use descriptive names indicating the scenario and expected outcome (e.g., `GetUserById_ReturnsUser_WhenUserExists`)

### Code Style
- **Braces:** Use Allman style (braces on new lines).
- **Indentation:** Use 4 spaces per indentation level.
- **Line Length:** Limit lines to 120 characters.
- **Usings:** Place `using` statements outside the namespace and remove unused usings.


### Required Before Each Commit
- Use Semantic Release Prefixes for Commits (e.g., `feat:`, `fix:`, `docs:`, `chore:`) with the following meanings:
  - `feat:` for new features
  - `fix:` for bug fixes
  - `docs:` for documentation changes
  - `chore:` for maintenance tasks (e.g., updating dependencies, refactoring)

## Repository Structure
- `Indiko.Maui.Controls.Markdown/`: Maui Library for the Markdown Viewer.
- `Indiko.Maui.Controls.Markdown.Sample/`: Sample Project for displaying the markdown viewer features.

## Key Guidelines
1. Follow c# best practices
2. Maintain existing code structure and organization
3. Use dependency injection patterns where appropriate
4. Write unit tests for new functionality and place it in the tests projects
5. **Adhere to these standards** when generating or modifying code.
6. **Prefer existing patterns** and conventions found in the solution.
7. **Generate code that is ready to use** and fits seamlessly into the current structure.
8. **Document any deviations** from these standards in pull requests or code reviews.

## Maui specific guidlines:

Use these general guidelines when doing anything for .NET MAUI.

## Page Lifecycle

Use `EventToCommandBehavior` from CommunityToolkit.Maui to handle page lifecycle events when using XAML.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:pageModels="clr-namespace:CommunityTemplate.PageModels"             
             xmlns:models="clr-namespace:CommunityTemplate.Models"
             xmlns:controls="clr-namespace:CommunityTemplate.Pages.Controls"
             xmlns:pullToRefresh="clr-namespace:Syncfusion.Maui.Toolkit.PullToRefresh;assembly=Syncfusion.Maui.Toolkit"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="CommunityTemplate.Pages.MainPage"
             x:DataType="pageModels:MainPageModel"
             Title="{Binding Today}">

    <ContentPage.Behaviors>
        <toolkit:EventToCommandBehavior
                EventName="NavigatedTo"
                Command="{Binding NavigatedToCommand}" />
        <toolkit:EventToCommandBehavior
                EventName="NavigatedFrom"
                Command="{Binding NavigatedFromCommand}" />
        <toolkit:EventToCommandBehavior
                EventName="Appearing"                
                Command="{Binding AppearingCommand}" />
    </ContentPage.Behaviors>
```

## Control Choices

* Prefer `Grid` over other layouts to keep the visual tree flatter
* Use `VerticalStackLayout` or `HorizontalStackLayout`, not `StackLayout`
* Use `CollectionView` or a `BindableLayout`, not `ListView` or `TableView`
* Use `BindableLayout` when the items source is expected to not exceed 20 items, otherwise use a `CollectionView`
* Use `Border`, not `Frame`
* Declare `ColumnDefinitions` and `RowDefinitions` inline like `<Grid RowDefinitions="*,*,40">`

## Custom Handlers

* Handler registration should be done in MauiProgram.cs within the builder.ConfigureHandlers method

```csharp
builder.ConfigureMauiHandlers(handlers =>
{
        handlers.AddHandler(typeof(Button), typeof(ButtonHandler));
}
```

## UI Components and Controls

- **Good Practice**: Ensure that any UI components or controls are compatible with .NET MAUI.
- **Bad Practice**: Using Xamarin.Forms-specific code unless there is a direct .NET MAUI equivalent.

```csharp
// Good Practice
public class CustomButton : Button
{
    // .NET MAUI specific implementation
}

// Bad Practice
public class CustomButton : Xamarin.Forms.Button
{
    // Xamarin.Forms specific implementation
}
```

## Handling Permissions

- **Good Practice**: Use `Permissions` API to request and check permissions.
- **Bad Practice**: Not handling permissions or using platform-specific code.

```csharp
// Good Practice
public async Task<PermissionStatus> CheckAndRequestLocationPermissionAsync()
{
    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    if (status != PermissionStatus.Granted)
    {
        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
    return status;
}

// Bad Practice
public async Task<bool> CheckAndRequestLocationPermissionAsync()
{
#if ANDROID
    var status = ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.AccessFineLocation);
    if (status != Permission.Granted)
    {
        ActivityCompat.RequestPermissions(MainActivity.Instance, new[] { Manifest.Permission.AccessFineLocation }, 0);
    }
    return status == Permission.Granted;
#elif IOS
    var status = CLLocationManager.Status;
    if (status != CLAuthorizationStatus.AuthorizedWhenInUse)
    {
        locationManager.RequestWhenInUseAuthorization();
    }
    return status == CLAuthorizationStatus.AuthorizedWhenInUse;
#endif
}
```

## Using Dependency Injection

- **Good Practice**: Use dependency injection to manage dependencies and improve testability.
- **Bad Practice**: Creating instances of dependencies directly within the class.

```csharp
// Good Practice
public class LocationService
{
    private readonly IGeolocation _geolocation;

    public LocationService(IGeolocation geolocation)
    {
        _geolocation = geolocation ?? throw new ArgumentNullException(nameof(geolocation));
    }

    public async Task<Location> GetLocationAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        return await _geolocation.GetLocationAsync(new GeolocationRequest(), token);
    }
}

// Bad Practice
public class LocationService
{
    private readonly IGeolocation _geolocation = new Geolocation();

    public async Task<Location> GetLocationAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        return await _geolocation.GetLocationAsync(new GeolocationRequest(), token);
    }
}
```

## Image Source Initialization

- **Good Practice**: Use dependency injection to initialize image sources.
- **Bad Practice**: Creating instances of image sources directly within the class.

```csharp
// Good Practice
public class CustomImageSource
{
    private readonly IImageService _imageService;

    public CustomImageSource(IImageService imageService)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        return await _imageService.LoadImageAsync(imageUrl, token);
    }
}

// Bad Practice
public class CustomImageSource
{
    private readonly IImageService _imageService = new ImageService();

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl)
    {
        return await _imageService.LoadImageAsync(imageUrl);
    }
}
```

## Handling Image Loading

- **Good Practice**: Implement proper error handling and logging when loading images.
- **Bad Practice**: Not handling exceptions or logging errors.

```csharp
// Good Practice
public async Task<ImageSource> GetImageSourceAsync(string imageUrl, CancellationToken token = default)
{
    try
    {
        token.ThrowIfCancellationRequested();
        return await _imageService.LoadImageAsync(imageUrl, token);
    }
    catch (Exception ex)
    {
        Trace.WriteLine($"Error loading image: {ex.Message}");
        return null;
    }
}

// Bad Practice
public async Task<ImageSource> GetImageSourceAsync(string imageUrl)
{
    return await _imageService.LoadImageAsync(imageUrl);
}
```

## Caching Image Sources

- **Good Practice**: Use caching mechanisms to improve performance and reduce network usage.
- **Bad Practice**: Not implementing caching for frequently used images.

```csharp
// Good Practice
public class CachedImageSource
{
    private readonly IImageService _imageService;
    private readonly IMemoryCache _cache;

    public CachedImageSource(IImageService imageService, IMemoryCache cache)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl, CancellationToken token = default)
    {
        if (_cache.TryGetValue(imageUrl, out ImageSource cachedImage))
        {
            return cachedImage;
        }

        token.ThrowIfCancellationRequested();
        var imageSource = await _imageService.LoadImageAsync(imageUrl, token);
        _cache.Set(imageUrl, imageSource);
        return imageSource;
    }
}

// Bad Practice
public class CachedImageSource
{
    private readonly IImageService _imageService;

    public CachedImageSource(IImageService imageService)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    public async Task<ImageSource> GetImageSourceAsync(string imageUrl)
    {
        return await _imageService.LoadImageAsync(imageUrl);
    }
}
```


1. Layout Tree:

A flatter visual tree is better since it reduces the number of measure and arrange passes needed to render the tree. For this reason, prefer a complex Grid layout over nesting multiple other layouts.

Do not nest scrollable controls (ScrollView, CollectionView, ListView) within each other unless they scroll in different directions. It is OK to nest within a RefreshView or similar pull-to-refresh control.

### Handling Layout Changes
- **Good Practice**: Override `OnSizeAllocated` to handle layout changes and adjust child elements accordingly.
- **Bad Practice**: Not handling layout changes or using event handlers for layout changes.

```csharp
// Good Practice
public class CustomLayout : StackLayout
{
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        // Adjust child elements based on new size
    }
}

// Bad Practice
public class CustomLayout : StackLayout
{
    public CustomLayout()
    {
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        // Adjust child elements based on new size
    }
}
```

2. Memory Leaks

### Avoid circular references on iOS and Catalyst

C# objects co-exist with a reference-counted world on Apple platforms, and so a C# object that subclasses `NSObject` can run into situations where they can accidentally live forever -- a memory leak. This situation does not occur on Android or Windows platforms.

Here is an example of a circular reference:

```csharp
class MyViewSubclass : UIView
{
    public UIView? Parent { get; set; }

    public void Add(MyViewSubclass subview)
    {
        subview.Parent = this;
        AddSubview(subview);
    }
}

//...

var parent = new MyViewSubclass();
var view = new MyViewSubclass();
parent.Add(view);
```

In this case:

* `parent` -> `view` via `Subviews`
* `view` -> `parent` via the `Parent` property
* The reference count of both objects is non-zero
* Both objects live forever

This problem isn't limited to a field or property. A similar situation may occur with C# events:

```csharp
class MyView : UIView
{
    public MyView()
    {
        var picker = new UIDatePicker();
        AddSubview(picker);
        picker.ValueChanged += OnValueChanged;
    }

    void OnValueChanged(object? sender, EventArgs e) { }

    // Use this instead and it doesn't leak!
    //static void OnValueChanged(object? sender, EventArgs e) { }
}
```

In this case:

* `MyView` -> `UIDatePicker` via `Subviews`
* `UIDatePicker` -> `MyView` via `ValueChanged` and `EventHandler.Target`
* Both objects live forever

A solution for this example, is to make `OnValueChanged` method `static`, which would result in a `null` `Target` on the `EventHandler` instance.

Another solution, would be to put `OnValueChanged` in a non-`NSObject` subclass:

```csharp
class MyView : UIView
{
    readonly Proxy _proxy = new();

    public MyView()
    {
        var picker = new UIDatePicker();
        AddSubview(picker);
        picker.ValueChanged += _proxy.OnValueChanged;
    }

    class Proxy
    {
        public void OnValueChanged(object? sender, EventArgs e) { }
    }
}
```

If the class subscribing to the events are not an `NSObject` subclass, we can also use a proxy (use weak references to the primary object). 

An example is a "view handler" which maps a "virtual view" to a "platform view".

The handler will have a readonly field for the proxy, and the proxy will manage the events between the platform view and the virtual view. The proxy should not have a reference to the handler as the handler does not take part in the events.

* The handler has a strong reference to the proxy
* The platform view has a strong reference to the proxy via the event handler
* The proxy has a _weak_ reference to the virtual view

```csharp
class DatePickerHandler : ViewHandler<IDatePicker, UIDatePicker>
{
    readonly Proxy proxy = new();

    protected override void ConnectHandler(UIDatePicker platformView)
    {
        proxy.Connect(VirtualView, picker);

        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(UIDatePicker platformView)
    {
        proxy.Disconnect(VirtualView, picker);

        base.DisconnectHandler(platformView);
    }

    void OnValueChanged() { }
    
    class Proxy
    {
        WeakReference<IDatePicker>? _virtualView;

        IDatePicker? VirtualView => _virtualView is not null && _virtualView.TryGetTarget(out var v) ? v : null;

        public void Connect(IDatePicker handler, UIDatePicker platformView)
        {
            _virtualView = new(virtualView);

            platformView.ValueChanged += OnValueChanged;
        }

        public void Disconnect(UIDatePicker platformView)
        {
            _virtualView = null;

            platformView.ValueChanged -= OValueChanged;
        }

        public void OnValueChanged(object? sender, EventArgs e)
        {
            VirtualView?.OnValueChanged();
        }
    }
}
```

This is the pattern used in most .NET MAUI handlers and other `UIView` subclasses to eliminate circular references.

3. Migration from Xamarin.Froms to Maui.net

Here are important things to know when upgrading code from Xamarin.Forms to .NET MAUI.

Changes to the projects *.csproj files:

- Convert any Xamarin.Forms class library project, Xamarin.iOS project, and Xamarin.Android project to SDK-style projects.
- Update the target framework in project files to net9.0-android and net9.0-ios, as required.
- Set <UseMaui>true</UseMaui> in project files.
- Remove NuGet package references incompatible with .NET 9 alternatives
- Add additional project properties, and remove project properties that aren't required.
- Replace the Xamarin.CommunityToolkit NuGet package with the .NET MAUI Community Toolkit NuGet package.
- Replace Xamarin.Forms compatible versions of the SkiaSharp NuGet packages with .NET MAUI compatible versions, if used.
- Remove references to the Xamarin.Essentials namespace, and replace the Xamarin.Forms namespace with the Microsoft.Maui and Microsoft.Maui.Controls namespaces.

## Layout Changes

- Review all Grid layouts and add explicit RowDefinitions and ColumnDefinitions to replace the auto-generated ones from Xamarin.Forms.
- Search for *AndExpand usage in StackLayout, HorizontalStackLayout, and VerticalStackLayout; remove or refactor them as they are treated as regular Fill options in .NET MAUI.
- Identify and update any RelativeLayout implementations. Refactor the layout to use Grid instead.
- Add implicit styles in the resource dictionaries to set default spacing (e.g., Grid.ColumnSpacing, Grid.RowSpacing, StackLayout.Spacing) to 6 unless a implicit styles already exist.
- Ensure that explicit sizing is applied to controls, as .NET MAUI enforces device-independent units exactly as specified.
- Review ScrollView usage in infinite layouts like VerticalStackLayout; constrain its size to enable proper scrolling behavior.
- Check usages of Frame and update them to use Border.

