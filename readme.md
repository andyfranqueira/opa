<h1>OPA: Online Parish Administration</h1>

A lightweight, CRM web application, designed for tracking church members & donations.  Admin accounts can manage member information, electronic payments, and track donations.  Members can access their own records online and view/update their information.
Designed to be extremely simple, the application can be easily tailored for use by any faith group, non-profit, club, etc.

Released under GNU General Public License v3.0

The Web.config file contains a few setting that should be populated before first use:

    <add key="org:Name" value="" />
    <add key="app:Name" value="" />
    <add key="app:Owner" value="" />

    <add key="smtp:Name" value="OPA Support" />
    <add key="smtp:Email" value="" />
    <add key="smtp:Host" value="" />
    <add key="smtp:Port" value="" />
    <add key="smtp:Account" value="" />
    <add key="smtp:Password" value="" />

As is, the application will compile and run with a local database under App_Data.  On initial creation an admin-owner account will be created with a random password.  Use the Forgot Password link to reset the password for this account.

As is, the application is designed to work with DonorBox/Stripe & Square.  This will require updating the DonorBox links and setting the appropriate API keys:
    
    <add key="url:DonorBox" value="" />
    <add key="key:Square" value="" />
    <add key="key:Stripe" value="" />


