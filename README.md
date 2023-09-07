# Bulwark.Auth (BETA)

Bulwark.Auth is an api based developer focused JWT authentication/authorization subsystem for any infrastructure. 

# Releases and contributing guidelines

### Releases

Pre-releases will be developed on the `beta` branches. Once the beta is stable it will be merged into `main` and a release will be generated

The docker tags for `beta` will be in this format: `1.0.11-beta.x`
The docker tags for `production` releases will be in this format: `1.0.11`

Most work for an upcoming release will do pull request against the `beta` branch. 

Depending on the change, for example important security fixes or bug fixes; it will be decided 
to have a PR on `main` or will hold off the PR on `beta` branch until the next release. 

If it is important bug fix or security update a release will happen ASAP. 

A releases will be targeted monthly, but if important feature(s) that are scheduled for the next upcoming release are not complete it is possible to push the release to the next release cycle. 
If there is no features schedule for a release it will be skipped.


### Contributions

- Each contributation will need a issue/ticket created to track the feature or bug fix before it will be considered
- The PR must pass all tests and be reviewed by an official maintainer
- Each PR must be linked to an issue/ticket, once the PR is merged the ticket will be auto closed
- Each feature/bugfix needs to have unit tests 
- Each feature must have the code documented inline 


# Key Features:
- Can use RS256, RS384, and RS512 asymmetric key signing on JWT tokens
- Plug and play key generation and rotation
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

# Configuring and Running Bulwark.Auth

Bulwark.Auth is best run using the official docker container found here: https://hub.docker.com/r/lateflip/bulwark.auth

For a k8s deployment please use the official helm chart found here: TBD

## Configuration
Configuration is done via environment variables. The following is a list of all available environment variables, any 
confidential values should use proper secrets management.

| Name                         | Description                                                                               | Value  | Example                                                                  | Mandatory |
|------------------------------|-------------------------------------------------------------------------------------------|--------|--------------------------------------------------------------------------|-----------|
| DB_CONNECTION                | The connection string to the mongo database                                               | string | mongodb://localhost:27017                                                | Yes       |
| DB_NAME_SEED                 | will append the seed onto the db name, this is needed if running many different instances | string | BulwarkAuth-{seed}                                                       | No        |
| DOMAIN                       | The domain name the service will be used for                                              | string | lateflip.io                                                              | Yes       |
| WEBSITE_NAME                 | The name of the website the service will be used for                                      | string | Late Flip                                                                | Yes       |
| VERIFICATION_URL             | The url of your application that will make the token verification call                    | string | https://localhost:3000/verify                                            | Yes       |
| FORGOT_PASSWORD_URL          | The url of your application that will use the forgot password call                        | string | https://localhost:3000/reset-password                                    | Yes       |
| MAGIC_LINK_URL               | The url of your application that will submit the magic code call                          | string | https://localhost:3000/magic-link                                        | Yes       |
| MAGIC_CODE_EXPIRE_IN_MINUTES | The number of minutes the magic code will be valid for                                    | int    | 10                                                                       | Yes       |
| EMAIL_SMTP                   | Whether or not to use smtp for sending emails                                             | bool   | true                                                                     | Yes       |
| EMAIL_SMTP_HOST              | The smtp host to use for sending emails                                                   | string | localhost                                                                | Yes       |
| EMAIL_SMTP_PORT              | The smtp port to use for sending emails                                                   | int    | 1025                                                                     | Yes       |
| EMAIL_SMTP_USER              | The smtp user to use for sending emails                                                   | string | user                                                                     | Yes       |
| EMAIL_SMTP_PASS              | The smtp pass to use for sending emails                                                   | string | pass                                                                     | Yes       |
| EMAIL_SMTP_SECURE            | Whether or not to use secure smtp for sending emails                                      | bool   | false                                                                    | Yes       |
| EMAIL_TEMPLATE_DIR           | The directory where the email templates are located                                       | string | src/bulwark-auth/email-templates                                         | Yes       |
| EMAIL_SEND_ADDRESS           | The email address to send emails from                                                     | string | admin@lateflip.io                                                        | Yes       |
| GOOGLE_CLIENT_ID             | The google client id to use for google authentication                                     | string | 651882111548-0hrg7e4o90q1iutmfn02qkf9m90k3d3g.apps.googleusercontent.com | No        |                                                                        |           |
| SERVICE_MODE                 | The service mode to run in only used for CI and tests                                     | string | test                                                                     | No        |

