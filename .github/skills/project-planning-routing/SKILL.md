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
- run the **Scope Check** (below) before escalating to Senior Solution Architect
- route coding or debugging changes to Executor Developer after the desired behavior is clear
- do not force a planning workflow for a focused technical question

### Scope Check — direct answer vs. Senior Solution Architect
Some triggers (code review, security review, refactoring advice, implementation recommendation) are listed under both "technical questions" and Senior Solution Architect. Use this check to decide which:

Answer directly (do NOT invoke Architect) when ALL of the following hold:
- the request concerns a single function, file, or narrowly-scoped snippet
- the fix or answer does not require changes across multiple modules or services
- there is no explicit ask for tradeoff analysis, risk assessment, or design validation
- the answer can be given in a few paragraphs without a structured multi-section report

Escalate to Senior Solution Architect when ANY of the following hold:
- the request spans multiple files, services, or system boundaries
- the user explicitly asks for tradeoffs, risk, scalability, or "is this a good idea"
- the decision affects architecture, data model, or long-term maintainability
- the user asks to validate or review a design, not just a snippet

If it's ambiguous which side of this line a request falls on, ask one clarifying question (e.g. "Is this just about the one function, or should I look at how it fits the wider system?") rather than guessing.

## Shared memory
This pipeline maintains a single `memory.md` file at the project root as the
**single source of truth every agent reads before acting** — no agent should
re-derive project context from the full codebase or conversation history
when `memory.md` already has the answer. `memory.md` is divided into
sections, and each section has exactly one owner who is allowed to write it:

| Section | Owner | Written |
|---|---|---|
| Goal / Chosen architecture / Assumptions / Risks | App Planner | Once, at plan approval |
| Architecture review notes | Senior Solution Architect | Appended, only when consulted |
| Task plan (task text + dependencies + acceptance criteria) | Task Breaker | Once, at breakdown |
| Task plan (checkbox state only) | Executor Developer | Toggled per task |
| Implementation log | Executor Developer | Appended, one entry per completed task |
| Status | Whoever last acted | Overwritten each time (one line) |

No agent overwrites a section it does not own. If an agent needs to change
something recorded by another agent (e.g. the Architect wants to change the
chosen architecture after tasks are already built against it), it appends a
note flagging the conflict instead of editing that section directly, and the
conflict is resolved with the user before anyone proceeds. If `memory.md`
doesn't exist yet when a later-stage agent is invoked, that is itself a
signal no plan has been approved — route back to App Planner rather than
improvising. Executor Developer prunes completed-phase detail out of the
implementation log as it goes (see its Memory maintenance rule), so the file
stays proportional to the number of phases rather than the number of tasks.

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
- an approved plan already exists (from App Planner output or user-supplied plan)
- the user wants the work divided into phases and tasks
- the user wants execution order and dependencies
- the user says things like:
  - "Break this into tasks"
  - "Turn this plan into implementation steps"
  - "Create a task list"
  - "Plan the execution order"

Note: "What should we do first?" is ambiguous between App Planner and Task Breaker.
Disambiguate using this rule: if no plan exists yet, treat it as App Planner (the
missing-plan case is more common and App Planner's own guardrails will stop and
ask if a plan already exists but wasn't shared). If a plan was already produced
or supplied in this conversation, treat it as Task Breaker.

Action:
- split work into phases
- create small executable tasks
- order by dependency
- define acceptance criteria
- note blockers, risks, and validation points

### Route to Senior Solution Architect when:
- the Scope Check above resolves to "escalate"
- the user asks for architecture review or technical validation
- the user wants tradeoff analysis
- the user asks whether a design is sound
- the user wants risk, scalability, maintainability, or security review at a system level
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
- if the user also asked for task breakdown or implementation as part of the same
  request, include a "Suggested handoff" note at the end of the review naming the
  next agent (Task Breaker or Executor Developer) rather than dropping that half
  of the request

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

Note: if no approved task or plan exists yet, Executor Developer's own guardrails
require it to stop and ask rather than improvise. To avoid an unnecessary round
trip, check before routing here: has App Planner or Task Breaker already produced
an approved plan/task in this conversation? If not, route to App Planner first.

Action:
- implement only the approved task
- stay within scope
- preserve prior context
- validate before completion
- record implementation memory
- do not explain unless asked

## Conflict resolution for compound requests
When a single message matches more than one route (e.g. "build X and tell me if
the schema is sound", or "review this architecture, then break it into tasks"):
1. Identify the most concrete, nearest-term part of the request as the primary
   route.
2. Complete that part fully.
3. Note the remaining part(s) explicitly as a follow-up, and name which agent
   would handle it next, instead of silently dropping it.
4. Never split a single response across two full agent output formats at once —
   finish one, then offer to proceed to the next.

## Fallback rule
If the user request is unclear or spans multiple stages:
- ask one clarifying question
- then route based on the clarified need
- never guess

## Final routing priority
Use this order when a request could plausibly match more than one *stage*:
1. App Planner
2. Task Breaker
3. Senior Solution Architect
4. Executor Developer

Only choose a later agent when the earlier stage is already complete and the
user's request matches that role. This priority governs pipeline-stage
ambiguity; it does not override the Scope Check (direct answer vs. Architect)
or the Conflict Resolution rule above, which apply first.

## Validation gate for the skill
Before routing, ask:
- Is the user asking for discovery, decomposition, architecture review, or implementation?
- Does the Scope Check resolve this to a direct answer instead of a full agent?
- Does the request match more than one route at once, requiring Conflict Resolution?
- Is the current stage already planned and approved?
- Is the request specific enough to route safely?

If not, ask one clarifying question instead of choosing the wrong agent.