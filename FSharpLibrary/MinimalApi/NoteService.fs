namespace MinimalApi

open System
open Microsoft.FSharp.Control
open Bogus

type Note = { Id: string; Content: string }

type Request =
    | GetNotes of AsyncReplyChannel<Response>

and Response = { Notes: Note seq; Count: int }

type NoteService() =
    let random = Random()

    let initNotes() =
        let count = random.Next(20, 983)
        let todoFaker =
            Faker<Note>()
                .CustomInstantiator(fun f ->
                    { Id = $"{f.IndexFaker + 1}"
                      Content = f.Lorem.Sentence() })

        let notes = todoFaker.Generate count

        // Add exactly 3 notes with "hello"
        for i in 0..2 do
            let randomIndex = random.Next(notes.Count)
            notes[randomIndex] <-
                { notes[randomIndex] with
                    Content = $"Hello! {Faker().Lorem.Sentence()}"
                }

        notes
        
    let agent = MailboxProcessor.Start(fun inbox ->
        let rec loop (notes: Note seq) = async {
            let! message = inbox.Receive()
            match message with
            | GetNotes reply ->
                let resp =
                    { Notes = notes
                      Count = Seq.length notes }
                reply.Reply(resp)
            return! loop notes
        }
        
        let state = initNotes()
        loop state
    )

    member _.Get() = agent.PostAndAsyncReply(GetNotes)
