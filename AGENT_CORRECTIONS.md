# Agent corrections

What the owner had to correct an agent on during one long working session with corvus-tawk-mcp
(16 to 18 October 2026), written down so the next agent starts where that one ended. People are named
by role only. Each item says what went wrong and what to do instead.

## Contents

- [Sending messages](#Sending%20messages)
- [Writing in the owner's voice](#Writing%20in%20the%20owner's%20voice)
- [Pictures and files](#Pictures%20and%20files)
- [Reporting what arrives](#Reporting%20what%20arrives)
- [Work outside WhatsApp](#Work%20outside%20WhatsApp)

<a id="Sending messages"></a>

## Sending messages

- **Queue, never set a draft.** The agent used `draft_message`; a second draft for the same chat was then refused
  because the first still sat in the input box. Use `send_message`, which queues the message for the owner to approve
  in tawk. The owner approves, edits or declines there.
- **A queued message cannot be changed.** When a draft needs another try, say which request id to decline and queue
  the new one; do not leave two versions waiting.
- **Find the exact thing before linking it.** The agent sent the wrong upload of a song. When a search returns
  several copies, show them and let the owner pick, or say which one was chosen and why.
- **Be specific about what something is.** An update said "admin panel" where the owner wanted it named plainly as
  the admin portal, so the reader could not mistake it for another part of the system.

<a id="Writing in the owner's voice"></a>

## Writing in the owner's voice

- **Read the voice and the contact first, then match the register the owner uses with that person.** Drafts to a
  close friend came out stiff and formal and were rejected twice. With people he writes to from his phone he uses
  full sentences in sentence case; quick lowercase lines are his terminal register. Use the one he uses with that
  person.
- **Afrikaans must be first-language Afrikaans,** never translated English, with the English mixed in the way he
  mixes it. Read it back as a native speaker before queueing.
- **Do not explain what the reader already knows.** A draft explained the owner's week of self-analysis to someone
  who had followed it all week.
- **No self-praise on his behalf.** Lines such as "it is spot on" about his own work were struck out. Say what the
  thing is and let the reader judge it.
- **Write for the reader's background.** For a reader who is not technical, say what it is in plain words (what it
  looks like, what it does for him) and leave the technical words out.
- **Say when the AI wrote it, if he asks.** He asked for a short line saying his AI wrote a message to one person;
  add it in the same language and register as the message.
- **The GLaDOS persona is brief.** When he asks for a GLaDOS reply: short, dry, sarcastic. The long versions were
  more than he wanted.

<a id="Pictures and files"></a>

## Pictures and files

- **The pictures are the point.** In a set of captioned screenshots the text arrived and many images failed. Check
  each message's status after it is approved (`read_messages`), report which failed, and do not say a set was
  delivered until each image shows delivered or read.
- **Send pictures one at a time.** Failures came when many sends went out together and tawk's per-minute write limit
  was hit. Space them out, and retry a failed one by itself.
- **Leave out what the reader should not see.** A desktop screenshot carried hardware details; say so before it
  goes, so the owner can drop it.

<a id="Reporting what arrives"></a>

## Reporting what arrives

- **Report new messages briefly, in the owner's terms,** and translate or explain only what helps (an emoji's likely
  meaning, a voice note's length). Offer a transcript rather than assuming one is wanted.
- **When another session sends on the owner's behalf, say so,** and say who it went to, so the owner can see the
  whole picture across sessions.

<a id="Work outside WhatsApp"></a>

## Work outside WhatsApp

The same session also set up the owner's desktop. The lessons that apply to any agent:

- **Ask what the owner likes before designing for him.** A first theme was built around rural imagery; he is
  "more of a cyberpunk dude": minimalism, code rain, neon, cyan over pink, and the raven as his own symbol.
- **Use real material when asked.** He asked for wallpapers to be found, not drawn.
- **Do what was asked, not a cleverer version.** A requested pulsing border was made "realistic" with flicker; he
  wanted the plain smooth pulse back. A requested frosted-glass blur did not show; when it did not work he wanted
  the earlier transparency back, not more attempts.
- **Use the real thing.** For the lock screen he wanted his own matrix-clock program shown, not a look-alike, and kept
  running after unlock.
- **Commit and push when the work is done,** and say which repositories were updated. Never put the client's or
  employer's name in his repositories.
- **Do not kill processes by a pattern that matches your own command line.** `pkill -f` with a path or class name
  that appears in the shell command ended the agent's own shell more than once; match on the process name or pid.
