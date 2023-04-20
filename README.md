# Bulwark.Auth

Bulwark.Auth is a simple JWT token authentication and account management service that easily fits into any 
infrastructure. 

# Key Features:
- Uses RS256 asymmetric cert signing on JWT tokens
- Deep token validation on the server side checks for revocation, expiration, and more
- Client side token validation can be used to reduce round trips to the server
- Uses a long lasting refresh token and short lived access token
- Bulwark.Auth does not need to be deployed on internal networks, it can be public facing
- Easy to use email templating based off razor templates
- Supports smtp configuration
- Sends out emails for account verification, forgot passwords, and magic links
- Supports passwordless authentication via magic links
- Supports password authentication
- Supports third party authentication via Google (more to come)
- Uses token acknowledgement to prevent replay attacks and supports multiple devices
- Account management and administration can be added by using: https://github.com/lateflip-io/Bulwark.Auth.Admin