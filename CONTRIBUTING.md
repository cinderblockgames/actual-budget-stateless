To contribute:
1. Fork the `develop` branch of this repository.
2. Add your environment variables through a `Properties/launchSettings.json` file or whichever other method your IDE supports.
   You'll want the following at a minimum:
    - ASPNETCORE_ENVIRONMENT=Development
    - API_URL
    - API_KEY
    - BUDGET_SYNC_ID (this will eventually be selected in app rather than set)
   
   Optional:
    - ASPNETCORE_URLS=http://+:5008
    - ACTUAL_LINK_URL
4. Make and test your changes.
5. Open a pull request with a description of what's been changed and how it was tested.

Note that this project adheres to the [SciActive Human Contribution Policy 2-NE](HUMAN-CONTRIBUTION-POLICY-2-NE.md).  No AI contributions of any kind are allowed.  This includes both code and text - your pull request should be written by you, not an LLM.  Anyone found to be using AI to contribute to this project will be permanently blocked from contributing.
