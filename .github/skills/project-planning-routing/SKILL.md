---
name: project-planning-routing
description: "Use for technical questions and software engineering work, including explanations, debugging, code review, architecture, security, performance, testing, app planning, task breakdown, and implementation."
---

# Project Planning and Execution Routing

Use this skill when the user asks for:
- technical explanations or how-to guidance
- debugging, error diagnosis, or troubleshooting
- code review, refactoring, testing, security, or performance advice
- app planning
- MVP scoping
- product idea validation
- feature discovery
- implementation task breakdown
- technical architecture review
- risk and design validation
- executing a task or feature

## Decision rules

### Route technical questions when:
- the user asks how code, a framework, API, library, or tool works
- the user asks why an error occurs or how to troubleshoot it
- the user asks for a code review, refactoring advice, testing strategy, security review, or performance analysis
- the user asks for a technical comparison or implementation recommendation

Action:
- identify the concrete technical question and the relevant code or project context
- answer directly when the question is sufficiently specific
- inspect the repository or ask one focused question when evidence is missing
- state assumptions and distinguish verified facts from recommendations
- route architecture, design, security, performance, and technical review questions to Senior Solution Architect
- route coding or debugging changes to Executor Developer after the desired behavior is clear
- do not force a planning workflow for a focused technical question

### Route to App Planner when:
- the user has a vague or rough idea
- the user wants a new app or feature planned from scratch
- the user asks for an MVP recommendation
- the user wants clarifying questions before finalizing the plan
- the user says things like:
  - "I want to build an app"
  - "I have an idea"
  - "Plan this for me"
  - "What should we build first?"
  - "Help me scope this MVP"

Action:
- summarize the goal
- ask only essential clarifying questions
- recommend the simplest viable MVP
- call out assumptions, constraints, and risks
- do not finalize a full architecture until the necessary questions are answered

### Route to Task Breaker when:
- the user already has a plan or proposal
- the user wants the work divided into phases and tasks
- the user wants execution order and dependencies
- the user says things like:
  - "Break this into tasks"
  - "Turn this plan into implementation steps"
  - "Create a task list"
  - "What should we do first?"
  - "Plan the execution order"

Action:
- split work into phases
- create small executable tasks
- order by dependency
- define acceptance criteria
- note blockers, risks, and validation points

### Route to Senior Solution Architect when:
- the user asks for architecture review or technical validation
- the user wants tradeoff analysis
- the user asks whether a design is sound
- the user wants risk, scalability, maintainability, or security review
- the user says things like:
  - "Review this architecture"
  - "Is this solution good?"
  - "What are the risks?"
  - "Should we use X or Y?"
  - "Validate this technical plan"

Action:
- assess architecture tradeoffs
- review risks and assumptions
- recommend practical technical direction
- validate design against constraints and delivery reality

### Route to Executor Developer when:
- the user wants implementation of an approved plan or task
- the user wants actual coding work performed
- the task list is already available
- the user says things like:
  - "Implement this task"
  - "Build this feature"
  - "Do the next task"
  - "Code this item"
  - "Execute this task list"

Action:
- implement only the approved task
- stay within scope
- preserve prior context
- validate before completion
- record implementation memory
- do not explain unless asked

## Fallback rule
If the user request is unclear or spans multiple stages:
- ask one clarifying question
- then route based on the clarified need
- never guess

## Final routing priority
Use this order:
1. App Planner
2. Task Breaker
3. Senior Solution Architect
4. Executor Developer

Only choose a later agent when the earlier stage is already complete and the user’s request matches that role.

## Validation gate for the skill
Before routing, ask:
- Is the user asking for discovery, decomposition, architecture review, or implementation?
- Is the current stage already planned and approved?
- Is the request specific enough to route safely?

If not, ask one clarifying question instead of choosing the wrong agent.
