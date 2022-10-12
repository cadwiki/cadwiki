# SSL security upgrades  
This weekend I've been thinking about improvements I can make to this site.  
The main idea that came to mind was security.  
   
## Overview
Since I started building this site, all the traffic has been over HTTP which isn't secure.  
So, this weekend I did some research into HTTPS and security certificates.  
I'm happy to announce that this site will redirect all HTTP traffic to HTTPS!   
Thie means that users can view my site from any link they want.    
And the site rules will automatically redirect them to the safer, HTTPS protocol.    


## Tech
I added a SSL security certificate to the domain host for this site by using CloudFlares namesevers.  
In addition, I added a redirect rule so now all HTTP traffic will be redirected to HTTPS.  
So now everyone should see the little grey lock to the left of the url in the browser address bar.  

## How I
Cloudflare does free SSL certificates, so even though I'm not hosting my website with them, they still hooked me up with a certifcate.  
They do this by giving out free namesevers that I can add to my domain which handle the certifcates for me.    
They make it very easy to add security certificates.  
They also offer 3 free redirect rules.  
It's a pretty neat service!  


## Links
[https://www.cloudflare.com/](https://www.cloudflare.com/)  

Here's how to create a redirect rule:  
[https://support.cloudflare.com/hc/en-us/articles/4729826525965-Configuring-URL-forwarding-or-redirects-with-Page-Rules](https://support.cloudflare.com/hc/en-us/articles/4729826525965-Configuring-URL-forwarding-or-redirects-with-Page-Rules)
