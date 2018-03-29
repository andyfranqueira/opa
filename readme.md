<h1>OPA: Online Parish Administration</h1>

A lightweight, CRM web application, designed for tracking church members, pledges &amp; donations.  Admin accounts can manage member information, pledges, donations, and electronic payments.  Members can access their own records online and view/update their information.  Designed to be extremely simple, the application can be easily tailored for use by any non-profit, faith group, club, etc.

Released under GNU General Public License v3.0

<h2>Objectives</h2>
<ul>
    <li>Manage group members and financial details with simplicity and accuracy.</li>
    <li>Enable members to use the system to manage their own details, pledges &amp; donations.</li>
    <li>Can be implemented and supported by semi-skilled volunteers for a low cost.</li>
    <li>Should be robust so that volunteers do not need to address issues with code changes.</li>
</ul>

<h2>Scope</h2>
<ul>
    <li>Functionality will cover member details and the tracking of pledges &amp; donations.</li>
    <li>The user experience will be simple and intuitive, with minimal steps and data entry.</li>
    <li>The application will use a mobile first philosophy.</li>
    <li>The application will be generic. Any customizations must be simple to enable and configure.</li>
    <li>Integrations make the application dependent on other systems and will be approached with caution.</li>
    <li>Any integrations must be optional and limited to market leading solutions designed for public integration.</li>
</ul>

<h2>Getting Started</h2>
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

Out-of-the-box the application is designed to work with DonorBox, Stripe &amp; Square.  Any combination can be activated by setting the donation platform links and the appropriate API keys:
    
    <add key="url:DonationForm" value="" />
    <add key="url:DonationOrgAcct" value=""/>
    <add key="url:DonationUserAcct" value=""/>
    <add key="key:Stripe" value="" />
    <add key="key:Square" value="" />
