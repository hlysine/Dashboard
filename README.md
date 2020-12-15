# Dashboard

A Windows desktop overlay integrating various services

Dashboard is still in early development stage! The software is unstable and undocumented. Use at your own risk!

## Download and Install

There are no releases yet. Download compiled binaries in [Actions](https://github.com/Henry-YSLin/Dashboard/actions).

The program is self-contained. No installation required. However, you need to manually set up the service connections. You should launch the app first, then look for `config.xml` in the same directory as the executable and fill in the required info (e.g. client IDs and secrets). When you launch the app again with the info filled in, Dashboard will initiate the OAuth flow and ask you to sign in to the services.

## Features

Dashboard is a desktop overlay consisting of various components, each connected to an external service. You can get quick info via the overlay and perform quick actions.

#### Integrated services (constantly updating)

- Spotify
  - See currently playing track
  - Play / pause
  - Switch to song radio
  - Save to / remove from library
- Google Tasks
  - View a task list
- Google Calendar
  - View events from all calendars
- Gmail
  - View all messages
- Weather
  - 5-day weather forecast at 3-hour intervals (Data source: [OpenWeatherMap](https://openweathermap.org/))
- Osu
  - View friends list
  
## Screenshot

![screenshot](https://user-images.githubusercontent.com/25472513/87216259-9e6e3e80-c370-11ea-91c0-c7d42acb51a2.png)
