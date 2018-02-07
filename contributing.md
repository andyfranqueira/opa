<h1>Contributing to OPA</h1>

<h2>Architecture</h2>
<ul>
    <li>The OPA web application utilizes an MVC architecture.</li>
    <li>Controllers should be thin, handle requests and route them accordingly.</li>
    <li>(View) Models should be used to abstract application entities and should contain the mapping logic where appropriate.</li>
    <li>Business logic should be consolidated in the corresponding classes.</li>
</ul>

<h2>Coding Conventions</h2>
<ul>
    <li>C# is the standard language for all custom .NET development.</li>
    <li>Code formatting will be managed via default Visual Studio formatting.</li>
    <li>All code should conform to the project’s StyleCop standards.</li>
    <li>Solutions and projects will implement a layered architecture.</li>
    <li>Microsoft’s published .NET design guidelines serve as the baseline standard.</li>
</ul>

<h3>Naming Conventions</h3>
<ul>
    <li>Microsoft’s published .NET design guidelines serve as the baseline standard.</li>
    <ul>
        <li>http://msdn.microsoft.com/en-us/library/vstudio/ms229045.aspx</li>
        <li>http://msdn.microsoft.com/en-us/library/vstudio/ms229043.aspx</li>
        <li>http://msdn.microsoft.com/en-us/library/vstudio/618ayhy6.aspx</li>
    </ul>
    <li>File and folder names should be in capitalized Camel Case without use of spaces or special characters.</li>
</ul>

<h3>HTML & CSS Conventions</h3>
<ul>
    <li>HTML element id and class names should:</li>
    <ul>
        <li>Use full descriptive words</li>
        <li>Define selectors using lowercase only</li>
        <li>Separate words using hyphens</li>
    </ul>
    <li>CSS should be in defined in external style sheets for all reusable classes.</li>
    <li>Avoid using HTML element id values for CSS declarations, class definitions are preferable.</li>
    <li>Color styles should be defined using lowercase Hex notation unless RGBA is required.</li>
    <li>User experience visual elements (CSS), will defer to the Bootstrap framework where needed.</li>
</ul>
