---
Order: 40
Title: Admin Only Sources
Description: Restrict access to sources based on Admin Only property
---

When Chocolatey GUI is used in conjunction with a Licensed version of Chocolatey, it is possible to restrict access to sources that have been configured on the system, using the Admin Only property.

If this setting has been set to true on a source, Chocolatey GUI will no longer show it within the application when it is being used by a non-admin user.

:::{.alert .alert-info}
**NOTE:**

This feature will only be supported in the Open Source version of Chocolatey GUI when using a licensed version of Chocolatey.  Open Source Chocolatey will always report all sources that have been configured, regardless of whether they have been marked as Admin Only.
:::

## Resources

Below is a short video which shows this feature in action:

<iframe width="700" height="506" src="https://www.youtube.com/embed/MBXnFdNMG28" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
