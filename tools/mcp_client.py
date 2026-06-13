#!/usr/bin/env python3
"""
Minimal MCP client for the local Unity MCP server.
Used to call tools/resources when native MCP tools are not yet loaded.
"""
import argparse
import json
import sys
import requests

DEFAULT_URL = "http://127.0.0.1:8080/mcp"


def get_session(url: str) -> str:
    r = requests.get(url, headers={"Accept": "application/json, text/event-stream"})
    return r.headers.get("mcp-session-id", "")


def initialize(url: str, session_id: str) -> None:
    headers = {
        "Accept": "application/json, text/event-stream",
        "Content-Type": "application/json",
        "mcp-session-id": session_id,
    }
    payload = {
        "jsonrpc": "2.0",
        "id": 0,
        "method": "initialize",
        "params": {
            "protocolVersion": "2024-11-05",
            "capabilities": {},
            "clientInfo": {"name": "mcp-client", "version": "1.0"},
        },
    }
    r = requests.post(url, headers=headers, json=payload, stream=True)
    for line in r.iter_lines():
        if line.startswith(b"data: "):
            return


def call(url: str, session_id: str, method: str, params: dict = None, msg_id: int = 1) -> dict:
    headers = {
        "Accept": "application/json, text/event-stream",
        "Content-Type": "application/json",
        "mcp-session-id": session_id,
    }
    payload = {"jsonrpc": "2.0", "id": msg_id, "method": method}
    if params is not None:
        payload["params"] = params
    r = requests.post(url, headers=headers, json=payload, stream=True)
    lines = []
    for line in r.iter_lines():
        line = line.decode("utf-8")
        if line.startswith("data: "):
            lines.append(json.loads(line[6:]))
    return lines[-1] if lines else {}


def main():
    parser = argparse.ArgumentParser(description="Call Unity MCP server")
    parser.add_argument("--url", default=DEFAULT_URL)
    parser.add_argument("--instance", default="unity-project@cf82f26f6952c089")
    parser.add_argument("method", help="MCP method, e.g. tools/call or resources/read")
    parser.add_argument("params", nargs="?", default="{}", help="JSON params object")
    args = parser.parse_args()

    session_id = get_session(args.url)
    initialize(args.url, session_id)

    if args.instance:
        call(args.url, session_id, "tools/call", {
            "name": "set_active_instance",
            "arguments": {"instance": args.instance},
        }, 1)

    params = json.loads(args.params)
    result = call(args.url, session_id, args.method, params, 2)
    print(json.dumps(result, indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
