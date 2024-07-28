import discord, time

async def clean(message: discord.message) -> bool:
    try:
        # List to store messages to be deleted
        bot_messages = []

        # Asynchronously iterate over the message history
        async for msg in message.channel.history(limit=950):
            if msg.author.bot:
                bot_messages.append(msg)

        # Delete the collected bot messages
        for msg in bot_messages:
            time.sleep(1)
            await msg.delete()
        
        return True
    except discord.Forbidden:
        return False
    except discord.HTTPException as e:
        print(f"Failed to delete a message: {e}")
        return False

    # await message.channel.send("Deleted all my messages!", delete_after=5)  # Optionally send a confirmation message
    # return True