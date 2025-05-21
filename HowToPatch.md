# Patching the IDA Pro 9.0 BETA

> [!NOTE]
> **Obligatory disclaimer:** this is for educational purposes only. I am not responsible for any damages caused by following this guide, or using any of the script(s) herein.

This guide prioritizes arm64 macOS, but may also work for other platforms.

---

## Step 1 - Patching dylibs

> [!NOTE]
> Repeat this step for both, `libida64.dylib` and `libida.dylib`, as both contain this signature check.

> [!WARNING]
> Make sure to create a backup of the libraries just in case something doesn't go correctly.

Start by opening the dylib in your favorite binary analysis software (or hex editor if you're doing a simple search for the public modulus sequence).

Search for usage of the string: `Signature decryption failed with code: %d`.

![image](https://gist.github.com/user-attachments/assets/8a01cab6-47a7-4542-9492-2e4486377862)

|Dylib|Public Modulus Location|
|---|---|
|`libida64.dylib`|`0x40bf44`|
|`libida.dylib`|`0x3f4b24`|

At the location, replace the fourth byte `0x5C` to `0xCB` (pictured below).

![image](https://gist.github.com/user-attachments/assets/ca1dcbe8-723f-43f2-adaf-8437be7ce3c7)

Finally, save the patched dylibs to their original location.

## Step 2

Use the attached script to generate an `ida.hexlic` for your copy.

## Troubleshooting

### Internal Error 30016

If encountering this error, navigate to `/Applications/IDA Professional 9.0.app/Contents/MacOS/plugins` and rename `arm_mac_user64.dylib` to `arm_mac_user64.dylib.bak`. 

If you so choose, you can also remove this file, as it does not seem to benefit the software in any way.

### "This application cannot be opened"

Try re-signing both dylibs:
```
codesign -f -s - --timestamp=none --all-architectures --deep "/Applications/IDA Professional 9.0.app/Contents/MacOS/libida.dylib"

codesign -f -s - --timestamp=none --all-architectures --deep "/Applications/IDA Professional 9.0.app/Contents/MacOS/libida64.dylib"
```


If that didn't work, try unquarantining IDA Pro using this command:

```
xattr -r -d com.apple.quarantine /Applications/IDA Professional 9.0.app
```

Alternatively, [try disabling System Integrity Protection](https://developer.apple.com/documentation/security/disabling_and_enabling_system_integrity_protection#3599244).

## References

* https://bbs.kanxue.com/thread-282846.htm
  * Also available at: https://archive.ph/6GiSe
