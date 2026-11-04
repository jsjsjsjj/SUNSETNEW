import 'package:flutter/material.dart';
import 'telas/login_tela.dart';

void main() {
  runApp(const SunsetApp());
}

class SunsetApp extends StatelessWidget {
  const SunsetApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'SUNSET',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        // Mesmo roxo do site.
        colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF6A1B9A)),
        useMaterial3: true,
      ),
      home: const LoginTela(),
    );
  }
}
