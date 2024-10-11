import asyncio
import aiofiles
import os.path
import readchar
from os import linesep as new_line, getcwd
from typing import AsyncGenerator, Generator


class TelePrompterConfig:
    def __init__(self, delay_in_ms: int = 200) -> None:
        self.delay_in_ms = delay_in_ms

    def update_delay(self, increment: int) -> None:
        new_delay = min(self.delay_in_ms + increment, 10000)
        new_delay = max(new_delay, 20)
        self.delay_in_ms = new_delay


async def read(path: str) -> AsyncGenerator[str, None]:
    async with aiofiles.open(path) as f:
        async for line in f:
            yield line


def format_line(line: str) -> Generator[str, None, None]:
    words = line.split(" ")
    line_length = 0
    for word in words:
        yield word + " "
        line_length += len(word) + 1

        if line_length > 70:
            yield new_line
            line_length = 0
    yield new_line


async def write_to_console(config: TelePrompterConfig, word: str) -> None:
    print(word, end="", flush=True)
    if word:
        await asyncio.sleep(config.delay_in_ms * 1e-3)


async def get_input(config: TelePrompterConfig) -> None:
    while True:
        key = await asyncio.to_thread(readchar.readchar)
        if key == "<":
            config.update_delay(10)
        elif key == ">":
            config.update_delay(-100)
        elif key == "X" or key == "x":
            break


async def main():
    path = os.path.join(getcwd(), "Quotes.txt")
    config = TelePrompterConfig()

    async def reading_task(config: TelePrompterConfig):
        async for line in read(path):
            for word in format_line(line):
                await write_to_console(config, word)

    input_task = asyncio.create_task(get_input(config))
    rtask = asyncio.create_task(reading_task(config))
    await asyncio.wait([input_task, rtask], return_when=asyncio.FIRST_COMPLETED)


asyncio.run(main())

