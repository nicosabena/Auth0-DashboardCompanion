#Auth0 Dashboard Companion
Try it now at [https://dashboardcompanion.azurewebsites.net/Apps/Try](https://dashboardcompanion.azurewebsites.net/Apps/Try)!

## Introduction

[Rules](https://auth0.com/docs/rules) in [Auth0](https://auth0.com/) can be written to act on a specific app (client), as shown in the "**Whitelist for a specific app**" or the "**Allow access during weekdays for a specific app**" [rule templates](https://manage.auth0.com/#/rules/new).

Those templates contains code that perform some action if the app name or id matches a specific string:
```javascript
function (user, context, callback) {

if (context.clientName === 'TheAppToCheckAccessTo') {
// do some logic
}

callback(null, user, context);
```
Sometimes the check is based on inequality:
```javascript
function (user, context, callback) {

if (context.clientName !== 'TheAppToCheckAccessTo') {
return callback(null, user, context);
}
// do something

callback(null, user, context);
```
##Relating apps and rules
While looking at the rule name (if aptly chosen) or code will usually provide a good indication to which app (if any) it applies, it can be difficult from an app perspective to identify all the rules that apply to a specific application.

To help with that, this application will scan the rules' scripts looking for known patterns, so that it can show applications and their related rules.

To match a rule to an app, we look for the following pattern in the script:
```
context.[clientName|clientId] [=== | !== | == | !=] 'expectedId/name'
```
> **Note:** Whitespace is ignored. Double quotes are also supported. There's also provision for escaped quotes in the app's name if you use them.

##Getting the information from Auth0
To get the registered rules and apps (clients), the application uses the [Management API v2](https://auth0.com/docs/api/v2).

The management API uses [JSON Web Tokens](https://jwt.io/) (JWT) to authenticate requests, so we need to get a valid token to access the required endpoints. Tokens are built in the API documentation page, and they take specific *scopes* (permissions). For this use case, we will need

 * `read:clients` and `read:client_keys` scopes to access the [/api/v2/clients](https://auth0.com/docs/api/v2#!/Clients/get_clients) endpoint to get the apps, and
 * `read:rules` scope for the [/api/v2/rules](https://auth0.com/docs/api/v2#!/Rules/get_rules) endpoint to get the rules.

##The app list view
With all the information gathered and the scripts analyzed, we can relate apps and rules. This is displayed in the default view like this:
![Apps view](http://i.imgur.com/T58lLcn.png)

For each application we show the list of rules that will execute, presented in their execution order. This list includes both common rules (that apply to every app) and rules that are specific for the app.

> **Common and disabled rules:** The user has the option to hide rules that are common to all apps and show only rules that are specific to one app.
>
By default, disabled rules are visible (greyed out), but can also be hidden if they are not needed.

>![checkboxes](http://i.imgur.com/wX9RqWQ.png)

####Linking to the Auth0 Dashboard
The user can click on any app or rule to view or edit the item in the Auth0 Dashboard. A configuration is required to indicate the proper destination(https://manage.auth0.com is the default when using cloud accounts, but will be different if Auth0 is deployed on-premises).


##The rules list view
Since we have the data available, we can also present the information organized by rules, showing which ones are commons to all apps and which ones are written for a specific app. This view is available by selecting the **Rules** tab:
![Rules view](http://i.imgur.com/vYjyKGM.png)

> **Note:** In the image we can see that we are given a warning when a rule references an app that wasn't found. This is usually caused by a misspelled name or id, or maybe the rule was meant for an app that was later deleted.

##Implementation details
The app is an [ASP.Net MVC 5](http://www.asp.net/mvc/mvc5) application as built by the standard template. It uses the [Auth0.ManagementApi NuGet package](https://www.nuget.org/packages/Auth0.ManagementApi) to call the Management API v2, and it also includes the [Auth0-ASPNET NuGet package](https://www.nuget.org/packages/Auth0-ASPNET/) to authenticate users (see below).

##Installation and configuration
###Access control
The application itself is protected by Auth0 authentication. Thus we need to create a new client app in our Auth0 account, select allowed connections and configure the callback url to enable `http://localhost:55250/LoginCallback.ashx` as an allowed url. Of course, if we deploy to a production environment, we need to update the url accordingly.

If we need special access control for the application, we can write a custom rule (such as "**White list for a specific app**").

###Configuring the app
A few values will need configuration in the `appSettings` section of `web.config`.

```xml
<add key="auth0:ClientId" value="[clientId for the app]"/>
<add key="auth0:ClientSecret" value="[clientSecret for the app]"/>
<add key="auth0:Domain" value="[your account domain]"/>

<!-- The dashboard domain is manage.auth0.com for cloud accounts,
but will be different for on-premises installations -->
<add key="auth0:DashboardDomain" value="manage.auth0.com"/>

<!-- this token should be built using the Api Token Generator at
https://auth0.com/docs/api/v2, with scopes read:rules,
read:clients and read:client_keys -->
<add key="ApiAccessToken" value="[the generated token]"/>
```

The first three values are the standard ones for every app protected by Auth0. The `DashboardDomain` allows us to properly generate links to the Dashboard for every app and rule in the user interface. Finally, the access token, generated in [https://auth0.com/docs/api/v2](https://auth0.com/docs/api/v2), is what gives the application the permissions to gather the required data.

## Try mode

There is a "try mode" that can be accessed at /Apps/Try that allows the user to directly
enter the account name and the API access token without using the configured values. 
You can try this at [https://dashboardcompanion.azurewebsites.net/Apps/Try](https://dashboardcompanion.azurewebsites.net/Apps/Try).

![Try mode](http://i.imgur.com/e0EuP9W.png)

##A few notes
####Rule stages
The application read rules for the `login_success` stage. If we have rules for other stages in the pipeline, we can read them all changing:

```c#
var rulesTask = apiClient.Rules.GetAllAsync(fields: "name,script,id,enabled,order");
```

to:
```c#
var rulesStages = new[] { "login_success", "login_failure", "pre_authorize", "user_registration", "user_blocked" };
var rulesTask =
    Task.WhenAll(
        rulesStages.Select(
            stage => apiClient.Rules.GetAllAsync(
                fields: "name,script,id,enabled,order", stage: stage)))
        .ContinueWith(t => t.Result.SelectMany(r => r));
```

####Script analysis
The parsing of the rule script to find a specific pattern is done with regular expressions in the `RulesScriptParser` class, with tests in the `RuleParsingTests` class.
This would be the place to put fixes if you use a different pattern to identify applications in scripts.

####Duplicated names
There is a possibility of inadvertedly using the same name for two (or more) different applications. This can cause that a rule that was originally thought for an specific app is now executing for more than one case.
A warning label will be shown in that situation:

![Duplicated name](http://i.imgur.com/d5geYLV.png)
