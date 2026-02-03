# TourPlanner Backlog

This file mirrors the active TourPlanner backlog so coding agents can work from the repository without querying Plane first.

- Plane project: `https://plane.w11core.cc/fh/projects/93a76523-78ae-4689-99ce-546e0b8e2801/issues/`
- Active issue range: `SWEN2-44` to `SWEN2-63`
- Current stack decision: frontend `React`, backend `C#`
- Canonical workflow state still lives in Plane

## SWEN2-44 Define implementation stack and project architecture

```gherkin
Feature: TourPlanner implementation stack and architecture

  Scenario: The approved implementation stack is documented
    Given the project requirements and teacher clarification
    When the architecture ticket is reviewed
    Then the frontend stack is specified as React
    And the backend stack is specified as C#

  Scenario: The application architecture is defined
    Given the initial project setup phase
    When the architecture decision is documented
    Then the layer structure UI, BL, and DAL is described
    And the MVVM approach for the frontend is described
    And at least one design pattern is identified for intentional use

  Scenario: Follow-up work can implement against a stable baseline
    Given the architecture ticket has been completed
    When engineers create implementation tickets or code changes
    Then the technical direction is unambiguous
    And major stack decisions do not need to be re-opened
```

## SWEN2-45 Bootstrap repository structure and shared engineering standards

```gherkin
Feature: Repository bootstrap and engineering standards

  Scenario: The empty repository is prepared for team development
    Given the repository starts without application structure
    When the bootstrap work is completed
    Then the repository contains clear frontend, backend, docs, and infrastructure areas

  Scenario: Shared engineering standards are established
    Given multiple engineers will work in the repository
    When the baseline configuration is added
    Then formatting and linting configuration is present
    And environment templates and ignore rules are present

  Scenario: Contribution guidance is available
    Given a new engineer opens the repository
    When they read the README
    Then setup and workflow guidance is documented
    And branch and contribution conventions are documented
```

## SWEN2-46 Set up React frontend application shell

```gherkin
Feature: React frontend application shell

  Scenario: The frontend application is initialized
    Given the project uses React for the frontend
    When the frontend shell is created
    Then the application can start locally
    And the project structure supports further feature work

  Scenario: Core frontend structure is available
    Given the initial frontend shell exists
    When an engineer inspects the frontend codebase
    Then routing is configured
    And layout structure is configured
    And the organization supports an MVVM-friendly frontend approach

  Scenario: Main screens have a host application shell
    Given the shell is running
    When a user navigates through the application
    Then the shell can host Tours, Tour Details, and Tour Logs views
```

## SWEN2-47 Set up backend service skeleton

```gherkin
Feature: Backend service skeleton

  Scenario: The backend project is initialized
    Given C# is the chosen backend stack
    When the backend skeleton is created
    Then the backend can be built and started locally

  Scenario: Layer responsibilities are separated
    Given the backend structure is in place
    When the codebase is reviewed
    Then domain, business logic, and data access concerns are separated

  Scenario: Basic service infrastructure is available
    Given the backend service is running
    When an engineer validates the service baseline
    Then dependency injection is wired
    And a basic health endpoint is available
```

## SWEN2-48 Set up PostgreSQL, ORM, and externalized configuration

```gherkin
Feature: PostgreSQL, ORM, and configuration setup

  Scenario: Local persistence is provisioned
    Given the project requires PostgreSQL
    When the persistence setup is completed
    Then a local PostgreSQL environment is available for development

  Scenario: Data access uses an ORM
    Given the backend stores tour data in PostgreSQL
    When persistence code is implemented
    Then an ORM is used for database interaction
    And direct string-built SQL is avoided for normal data access

  Scenario: Configuration is externalized
    Given the application needs connection strings and API keys
    When configuration is reviewed
    Then configuration values are stored outside source code
    And environment-specific values can be changed without code edits
```

## SWEN2-49 Model core Tour and Tour Log domain entities

```gherkin
Feature: Tour and Tour Log domain modeling

  Scenario: Tour entities contain required attributes
    Given the project domain is being modeled
    When the Tour entity is defined
    Then all required tour attributes are represented
    And image support is represented

  Scenario: Tour Log entities contain required attributes
    Given the project domain is being modeled
    When the Tour Log entity is defined
    Then all required log attributes are represented

  Scenario: Domain behavior supports assignment requirements
    Given the Tour domain model exists
    When the model is reviewed
    Then computed tour attributes are defined
    And domain-specific exceptions are used instead of implementation-specific exceptions
```

## SWEN2-50 Implement database schema and repositories for Tours and Tour Logs

```gherkin
Feature: Database schema and repositories for Tours and Tour Logs

  Scenario: The database schema supports the required data
    Given Tour and Tour Log models have been defined
    When the persistence schema is created
    Then the database can store Tours and Tour Logs correctly

  Scenario: Repository access is available through the DAL
    Given the persistence layer is implemented
    When business logic needs tour data
    Then repository interfaces are available
    And concrete repository implementations provide the required data access

  Scenario: Tour data is stored through the persistence layer
    Given an engineer reviews the architecture boundaries
    When tour data is created, updated, or deleted
    Then the DAL is responsible for persistence operations
```

## SWEN2-51 Integrate OpenRouteService for route retrieval

```gherkin
Feature: OpenRouteService integration

  Scenario: Route information can be retrieved for a tour
    Given a user provides route-relevant tour input
    When the application requests route data
    Then the OpenRouteService Directions API is called
    And route metadata is returned for application use

  Scenario: Route metadata can be used by the system
    Given route data has been retrieved successfully
    When the backend processes the response
    Then the relevant route information is stored or exposed to the application

  Scenario: External API failures are handled safely
    Given the external route service may fail or return invalid data
    When such a failure occurs
    Then the application handles the failure gracefully
    And the failure does not crash the system
```

## SWEN2-52 Integrate Leaflet map rendering in the frontend

```gherkin
Feature: Leaflet map rendering

  Scenario: Tours can be displayed on a map
    Given a tour with route information exists
    When the user opens the map view
    Then the route is rendered using Leaflet

  Scenario: Tour Details include map content
    Given a user selects a tour
    When the Tour Details view is shown
    Then relevant map content for that tour is visible

  Scenario: Map rendering integrates with the frontend application
    Given the React frontend is running
    When the map feature is inspected
    Then the Leaflet integration works within the application shell
```

## SWEN2-53 Implement Tours list and create/edit/delete workflow

```gherkin
Feature: Tours list and CRUD workflow

  Scenario: Tours are visible in a list
    Given Tours exist in the system
    When the user opens the Tours view
    Then Tours are displayed in a list view

  Scenario: A user can create, modify, and delete Tours
    Given the Tours workflow is implemented
    When the user performs create, edit, or delete actions
    Then the UI and backend both support those operations

  Scenario: Invalid user input is validated safely
    Given a user enters invalid Tour data
    When the data is submitted
    Then validation feedback is provided
    And the application does not crash
```

## SWEN2-54 Implement Tour Details view with computed attributes and map

```gherkin
Feature: Tour Details view

  Scenario: A selected Tour shows all relevant details
    Given a user selects a Tour
    When the Tour Details view is displayed
    Then all Tour attributes are shown

  Scenario: Computed Tour attributes are visible
    Given a Tour has computed attributes
    When the Tour Details view is displayed
    Then computed attributes are shown alongside the stored attributes

  Scenario: The Tour Details view includes map output
    Given a selected Tour has route information
    When the Tour Details view is displayed
    Then the corresponding map content is shown
```

## SWEN2-55 Implement Tour Log list and create/edit/delete workflow

```gherkin
Feature: Tour Log list and CRUD workflow

  Scenario: Logs for a selected Tour are visible
    Given a Tour has associated logs
    When the user selects that Tour
    Then all Tour Logs for the selected Tour are shown in a list view

  Scenario: A user can create, modify, and delete Tour Logs
    Given the Tour Log workflow is implemented
    When the user performs create, edit, or delete actions for a log
    Then the UI and backend both support those operations

  Scenario: Invalid Tour Log input is handled safely
    Given a user enters invalid Tour Log data
    When the data is submitted
    Then validation feedback is provided
    And the application does not crash
```

## SWEN2-56 Implement full-text search across Tours, Tour Logs, and computed attributes

```gherkin
Feature: Full-text search across Tours and Tour Logs

  Scenario: Tour data is searchable
    Given Tours exist in the system
    When the user enters a search query
    Then full-text search is performed across Tours

  Scenario: Tour Logs and computed attributes are included in search
    Given Tour Logs and computed Tour attributes exist
    When the user enters a search query
    Then matching Tour Logs are considered in the search
    And matching computed Tour attributes are considered in the search

  Scenario: The Tour list reflects the current search
    Given a search query has been applied
    When search results are returned
    Then the visible Tour list reflects the current search result set
```

## SWEN2-57 Implement import and export for tour data

```gherkin
Feature: Tour data import and export

  Scenario: Tour data can be exported
    Given Tour data exists in the system
    When the user triggers an export operation
    Then the system exports Tour data in the defined format

  Scenario: Tour data can be imported
    Given an import file in the supported format is available
    When the user triggers an import operation
    Then the system imports the Tour data

  Scenario: Invalid import content is handled safely
    Given an import file is invalid or incomplete
    When the import operation is attempted
    Then the system reports the problem safely
    And the application does not crash
```

## SWEN2-58 Implement reusable frontend component and responsive UI behavior

```gherkin
Feature: Reusable frontend component and responsive UI behavior

  Scenario: A reusable UI component exists
    Given repeated UI behavior or presentation exists in the frontend
    When the component is implemented
    Then at least one reusable UI component is available

  Scenario: UI state is correctly bound to frontend view model logic
    Given the frontend uses an MVVM-friendly approach
    When underlying state changes
    Then the visible UI updates correctly through binding mechanisms

  Scenario: The UI responds to window size changes
    Given the application is opened on different window sizes
    When the viewport size changes
    Then the UI remains usable and adapts appropriately
```

## SWEN2-59 Add application logging, error handling, and validation strategy

```gherkin
Feature: Logging, error handling, and validation strategy

  Scenario: Technical failures are logged
    Given runtime exceptions or technical errors occur
    When the application handles the failure
    Then useful technical information is logged

  Scenario: Validation is applied at system boundaries
    Given data enters the system through frontend or backend boundaries
    When invalid data is provided
    Then the system validates the input consistently

  Scenario: Layer-specific error handling is preserved
    Given an implementation-specific failure occurs in a lower layer
    When the failure is propagated upward
    Then layer-appropriate exceptions are used where necessary
    And implementation-specific details are not leaked unnecessarily
```

## SWEN2-60 Add automated tests and reach minimum unit test coverage target

```gherkin
Feature: Automated tests for TourPlanner

  Scenario: The project meets the minimum unit test requirement
    Given the assignment requires at least 20 unit tests
    When the test suite is reviewed
    Then the project contains at least 20 unit tests

  Scenario: Tests provide meaningful coverage
    Given automated tests have been added
    When the test suite is assessed
    Then the tests cover useful units of behavior
    And duplicate or low-value tests are avoided

  Scenario: Testing decisions can be documented
    Given the test strategy has been implemented
    When the final protocol is prepared
    Then the rationale for the chosen tests can be explained clearly
```

## SWEN2-61 Design and document the mandatory unique feature

```gherkin
Feature: Unique feature design and documentation

  Scenario: A unique feature is selected intentionally
    Given the assignment requires a mandatory unique feature
    When the design task is completed
    Then the chosen feature provides value beyond the base specification

  Scenario: The unique feature scope is defined before implementation
    Given the unique feature has been selected
    When the design ticket is reviewed
    Then expected behavior is documented
    And technical impact is documented
    And acceptance criteria are documented

  Scenario: The feature is ready for implementation planning
    Given the design ticket is complete
    When engineers start implementation work
    Then the unique feature scope is clear enough to build against
```

## SWEN2-62 Implement the mandatory unique feature

```gherkin
Feature: Mandatory unique feature implementation

  Scenario: The approved unique feature is implemented
    Given the unique feature design has been completed
    When implementation work is finished
    Then the application provides the approved unique behavior

  Scenario: The feature integrates with the existing system
    Given the core TourPlanner application exists
    When the unique feature is used
    Then it integrates cleanly with the existing architecture

  Scenario: The feature is suitable for testing and documentation
    Given the unique feature has been implemented
    When the project is reviewed
    Then the feature can be tested
    And the feature can be documented in the protocol
```

## SWEN2-63 Create protocol and hand-in documentation artifacts

```gherkin
Feature: Protocol and hand-in documentation

  Scenario: The protocol describes the architecture
    Given the final project documentation is prepared
    When a reviewer reads the architecture section
    Then layer responsibilities are documented
    And class diagrams are included

  Scenario: The protocol describes behavior and UX
    Given the final project documentation is prepared
    When a reviewer reads the analysis and UX sections
    Then use cases are documented
    And sequence diagrams are documented
    And UX wireframes are documented

  Scenario: The protocol covers project decisions and hand-in requirements
    Given the final project documentation is prepared
    When a reviewer inspects the remaining documentation sections
    Then library decisions and design pattern usage are documented
    And testing decisions and lessons learned are documented
    And tracked time, repository link, and the unique feature description are included
```
