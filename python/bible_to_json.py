import requests
import json
import os
from biblebooks import BibleBooks  # Import the BibleBooks enum


def fetch_bible_data(book: BibleBooks, chapter: int, verse: int = None):
    """
    Fetches Bible data for a given book, chapter, and optional verse.
    Writes the JSON data to a file in a pretty format.

    Parameters:
        book (BibleBooks): The book of the Bible (e.g., BibleBooks.GENESIS).
        chapter (int): The chapter number.
        verse (int, optional): The verse number (default is None).
    """

    base_url = "https://cdn.jsdelivr.net/gh/wldeh/bible-api/bibles/en-lsv/books"
    if verse:
        url = f"{base_url}/{book.value}/chapters/{chapter}/verses/{verse}.json"
    else:
        url = f"{base_url}/{book.value}/chapters/{chapter}.json"

    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()

        # Check if the data contains a "data" key with a list of verses
        if "data" in data and isinstance(data["data"], list):
            for item in data["data"]:
                if "text" in item:
                    # Remove square brackets from the "text" field
                    item["text"] = item["text"].replace(
                        "[", "").replace("]", "")
        elif isinstance(data, dict) and "text" in data:  # If the data is a single verse
            data["text"] = data["text"].replace("[", "").replace("]", "")

        # Ensure the directory exists
        output_dir = f"json/{book.value}"
        os.makedirs(output_dir, exist_ok=True)

        # Write the cleaned JSON data to a file with pretty formatting
        output_file = f"{output_dir}/{book.name.lower()}_chapter_{chapter}{'_verse_' + str(verse) if verse else ''}.json"
        with open(output_file, "w", encoding="utf-8") as file:
            json.dump(data, file, indent=4)

        print(
            f"📁 JSON data has been written to '{output_file}' in a pretty format.")
    else:
        print("❌ Error:", response.status_code)
