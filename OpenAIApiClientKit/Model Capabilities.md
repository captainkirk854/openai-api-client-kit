# Model Capability Registry
This registry defines a structured, extensible framework for describing and comparing AI models across multiple generations. It includes a unified capability taxonomy, a strict JSON schema, and complete registry data for all base models and the GPT‑5.x families.

# Capability Taxonomy
The registry uses a consistent set of 16 weighted capability tags, grouped into four high‑level domains. Each tag uses a 0–5 scale, 
where 0 means unsupported and 5 indicates a primary strength.

## Core - Fundamental cognitive and multimodal capabilities
- Reasoning — depth, logic, multi‑step problem solving
- Text — general text generation quality
- Chat — conversational alignment and instruction following
- Vision — ability to process images
- AudioIn — speech/audio input
- AudioOut — speech/audio output

## Advanced - Higher-order behavioural and structured-output capabilities
- Critic — ability to critique, evaluate, and refine content
- Editor — ability to rewrite, transform, or restructure content
- JSONMode — structured output reliability
- Embedding — vector embedding generation
- ImageGeneration — generative image capabilities

## Performance - Speed, throughput, and model capability tier
- FastInference — latency and throughput
- HighPerformance — overall capability tier

## Operational - Cost, safety, and deployment characteristics
- LowCost — cost‑efficiency
- Moderation — safety and filtering strength
- OpenWeight — availability of open‑weight versions

# JSON Schema
A strict JSON schema defines the structure of the registry. It enforces:
- Required capability groups
- Required tags within each group
- Integer weights between 0 and 5
- No unknown fields
- Extensibility for future model families

Each model entry follows this structure:
```
{
  "name": "model-name",
  "family": "model-family",
  "capabilities": {
    "core": { ... },
    "advanced": { ... },
    "performance": { ... },
    "operational": { ... }
  }
}
```

# The Base Models Registry

This comprehensive Model Capability Registry is suitable for:
- Capability‑based routing
- Model selection engines
- Benchmarking
- Documentation
- Orchestration frameworks

A complete registry was generated for models in the following model families:
- GPT‑5.x Families
- GPT‑4.1 family
- GPT‑4o family
- O‑series
- GPT‑3.5 family
- Embedding models
- Audio models
- Image models
- Moderation models

- Each model includes all 16 weighted capability tags

- Each family includes:
    - Base
    - Pro
    - Mini
    - Nano
    - Vision
    - Audio

- All models were assigned weights so that: GPT‑5.2 > GPT‑5.1 > GPT‑5.0 > GPT‑4.x

- Vision and Audio variants received enhanced multimodal scores.

# Heatmaps
## Capability Heatmap

| Capability     | Core | Advanced | Performance | Operational |
|----------------|------|----------|-------------|-------------|
| **Reasoning** | ●●●●● | ●●●○○ | ●●●●○ | ●○○○○ |
| **Text** | ●●●●● | ●●●○○ | ●●●○○ | ●○○○○ |
| **Chat** | ●●●●● | ●●●○○ | ●●●○○ | ●○○○○ |
| **Vision** | ●●●●○ | ●●○○○ | ●●●○○ | ●○○○○ |
| **AudioIn** | ●●●○○ | ●○○○○ | ●●○○○ | ●○○○○ |
| **AudioOut** | ●●●○○ | ●○○○○ | ●●○○○ | ●○○○○ |
| **Critic** | ●●○○○ | ●●●●○ | ●●○○○ | ●○○○○ |
| **Editor** | ●●○○○ | ●●●●○ | ●●○○○ | ●○○○○ |
| **JSONMode** | ●●●○○ | ●●●●● | ●●●○○ | ●○○○○ |
| **Embedding** | ●○○○○ | ●●●○○ | ●●●○○ | ●○○○○ |
| **ImageGeneration** | ●○○○○ | ●●●●○ | ●●○○○ | ●○○○○ |
| **FastInference** | ●●○○○ | ●○○○○ | ●●●●● | ●●○○○ |
| **HighPerformance** | ●●●○○ | ●●○○○ | ●●●●● | ●●○○○ |
| **LowCost** | ●○○○○ | ●○○○○ | ●●○○○ | ●●●●● |
| **Moderation** | ●○○○○ | ●○○○○ | ●●○○○ | ●●●●● |
| **OpenWeight** | ●○○○○ | ●○○○○ | ●○○○○ | ●●●●● |

The heatmap uses a simple scale:
- ●●●●● = strongest relevance
- ●●●●○ = high relevance
- ●●●○○ = moderate relevance
- ●●○○○ = low relevance
- ●○○○○ = minimal relevance
- ○○○○○ = not applicable

This is not model‑specific — it’s a taxonomy‑level heatmap showing how each capability group relates to the 16 tags.

How to Use This Heatmap
- It gives a taxonomy‑level overview, not model‑level scores.
- It helps readers understand which capability groups matter most for each tag.
- It visually reinforces why the 16‑tag structure is balanced and expressive.

## Family‑Comparison Heatmap (All Model Families)

This table compares the major families:
- GPT‑3.5
- GPT‑4.1
- GPT‑4o
- O‑Series
- GPT‑5.0
- GPT‑5.1
- GPT‑5.2

| Capability Tag | GPT‑3.5 | GPT‑4.1 | GPT‑4o | O‑Series | GPT‑5.0 | GPT‑5.1 | GPT‑5.2 |
|----------------|---------|---------|--------|----------|----------|----------|----------|
| **Reasoning** | ●●○○○ | ●●●●○ | ●●●●○ | ●●○○○ | ●●●●○ | ●●●●● | ●●●●● |
| **Text** | ●●●○○ | ●●●●○ | ●●●●● | ●●●○○ | ●●●●● | ●●●●● | ●●●●● |
| **Chat** | ●●●○○ | ●●●●○ | ●●●●● | ●●●○○ | ●●●●● | ●●●●● | ●●●●● |
| **Vision** | ○○○○○ | ●●○○○ | ●●●●● | ●○○○○ | ●●●○○ | ●●●●○ | ●●●●● |
| **AudioIn** | ○○○○○ | ●○○○○ | ●●●●○ | ●○○○○ | ●●●○○ | ●●●●○ | ●●●●● |
| **AudioOut** | ○○○○○ | ●○○○○ | ●●●●○ | ●○○○○ | ●●●○○ | ●●●●○ | ●●●●● |
| **Critic** | ●○○○○ | ●●●○○ | ●●●●○ | ●○○○○ | ●●●●○ | ●●●●● | ●●●●● |
| **Editor** | ●○○○○ | ●●●○○ | ●●●●○ | ●○○○○ | ●●●●○ | ●●●●● | ●●●●● |
| **JSONMode** | ●○○○○ | ●●●●○ | ●●●●● | ●●○○○ | ●●●●● | ●●●●● | ●●●●● |
| **Embedding** | ●●●○○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●●●○ | ●●●●● | ●●●●● |
| **ImageGeneration** | ○○○○○ | ●○○○○ | ●●●●● | ○○○○○ | ●●●●○ | ●●●●● | ●●●●● |
| **FastInference** | ●●●●○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●● |
| **HighPerformance** | ●●○○○ | ●●●●○ | ●●●●● | ●●○○○ | ●●●●○ | ●●●●● | ●●●●● |
| **LowCost** | ●●●●○ | ●●○○○ | ●●○○○ | ●●●●● | ●●●○○ | ●●●○○ | ●●●○○ |
| **Moderation** | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ | ●●●●○ | ●●●●● | ●●●●● |
| **OpenWeight** | ○○○○○ | ○○○○○ | ○○○○○ | ●●●●● | ○○○○○ | ○○○○○ | ○○○○○ |

The scale:
- ●●●●● = strongest
- ●●●●○ = very strong
- ●●●○○ = strong
- ●●○○○ = moderate
- ●○○○○ = weak
- ○○○○○ = none

How to read this heatmap
- GPT‑3.5 is weak across most advanced and multimodal capabilities.
- GPT‑4.1 is strong in reasoning and structured output but not fully multimodal.
- GPT‑4o is the strongest pre‑GPT‑5 family, especially in multimodality.
- O‑Series is optimised for cost and speed, not capability.
- GPT‑5.0 introduces across‑the‑board improvements.
- GPT‑5.1 strengthens reasoning, multimodality, and JSON reliability.
- GPT‑5.2 is the top‑tier family across all 16 tags.


## Tier‑Level Heatmap (GPT‑5.0 / 5.1 / 5.2)

| Capability Tag | 5.0‑Pro | 5.0‑Base | 5.0‑Mini | 5.0‑Nano | 5.0‑Vision | 5.0‑Audio |
|----------------|---------|----------|----------|----------|------------|-----------|
| **Reasoning** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●○○ | ●●●○○ |
| **Text** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ |
| **Chat** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ |
| **Vision** | ●●●○○ | ●●○○○ | ●○○○○ | ○○○○○ | ●●●●○ | ●●●○○ |
| **AudioIn** | ●●●○○ | ●●○○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●●●○ |
| **AudioOut** | ●●●○○ | ●●○○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●●●○ |
| **Critic** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●○ |
| **Editor** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●○ |
| **JSONMode** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **Embedding** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●○○ | ●●●○○ |
| **ImageGeneration** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●● | ●●●○○ |
| **FastInference** | ●●●●○ | ●●●●○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●●○ |
| **HighPerformance** | ●●●●○ | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ |
| **LowCost** | ●●○○○ | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ | ●●○○○ |
| **Moderation** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●○ |
| **OpenWeight** | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ |

| Capability Tag | 5.1‑Pro | 5.1‑Base | 5.1‑Mini | 5.1‑Nano | 5.1‑Vision | 5.1‑Audio |
|----------------|---------|----------|----------|----------|------------|-----------|
| **Reasoning** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ |
| **Text** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Chat** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Vision** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●● | ●●●●○ |
| **AudioIn** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●● |
| **AudioOut** | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●● |
| **Critic** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **Editor** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **JSONMode** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Embedding** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ |
| **ImageGeneration** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●○ |
| **FastInference** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **HighPerformance** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **LowCost** | ●●○○○ | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ | ●●○○○ |
| **Moderation** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **OpenWeight** | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ |

| Capability Tag | 5.2‑Pro | 5.2‑Base | 5.2‑Mini | 5.2‑Nano | 5.2‑Vision | 5.2‑Audio |
|----------------|---------|----------|----------|----------|------------|-----------|
| **Reasoning** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Text** | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **Chat** | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **Vision** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **AudioIn** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **AudioOut** | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **Critic** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Editor** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **JSONMode** | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **Embedding** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **ImageGeneration** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **FastInference** | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **HighPerformance** | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **LowCost** | ●●○○○ | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ | ●●○○○ |
| **Moderation** | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **OpenWeight** | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ |

The scale:
- ●●●●● = strongest
- ●●●●○ = very strong
- ●●●○○ = strong
- ●●○○○ = moderate
- ●○○○○ = weak
- ○○○○○ = none

This heatmap gives you a clear, visual hierarchy inside each GPT‑5 family, showing exactly how Pro/Base/Mini/Nano compare and how Vision/Audio 
variants get boosted on multimodal tags.


## Combined Mega‑Heatmap (All Families × All Tiers)

| Capability Tag | 3.5‑Base | 4.1‑Pro | 4.1‑Base | 4o‑Pro | 4o‑Base | O‑Pro | O‑Base | 5.0‑Pro | 5.0‑Base | 5.0‑Mini | 5.0‑Nano | 5.0‑Vision | 5.0‑Audio | 5.1‑Pro | 5.1‑Base | 5.1‑Mini | 5.1‑Nano | 5.1‑Vision | 5.1‑Audio | 5.2‑Pro | 5.2‑Base | 5.2‑Mini | 5.2‑Nano | 5.2‑Vision | 5.2‑Audio |
|----------------|----------|---------|----------|--------|---------|-------|--------|----------|-----------|-----------|-----------|-------------|------------|----------|-----------|-----------|-----------|-------------|------------|----------|-----------|-----------|-----------|-------------|------------|
| **Reasoning** | ●●○○○ | ●●●●○ | ●●●○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●○○ | ●●●○○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Text** | ●●●○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **Chat** | ●●●○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **Vision** | ○○○○○ | ●●○○○ | ●●○○○ | ●●●●● | ●●●○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●○○○ | ●○○○○ | ○○○○○ | ●●●●○ | ●●●○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●● | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **AudioIn** | ○○○○○ | ●○○○○ | ●○○○○ | ●●●●○ | ●●●○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●○○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●● | ●●●●● | ●●●○○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **AudioOut** | ○○○○○ | ●○○○○ | ●○○○○ | ●●●●○ | ●●●○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●○○○ | ●○○○○ | ○○○○○ | ●●●○○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●● | ●●●●● | ●●●○○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● |
| **Critic** | ●○○○○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●○○ | ●○○○○ | ○○○○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **Editor** | ●○○○○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●○○ | ●○○○○ | ○○○○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **JSONMode** | ●○○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●○○○ | ●○○○○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **Embedding** | ●●●○○ | ●●●●○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●●●○ | ●●●●○ |
| **ImageGeneration** | ○○○○○ | ●○○○○ | ●○○○○ | ●●●●● | ●●●○○ | ○○○○○ | ○○○○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●● | ●●●○○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●○ | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **FastInference** | ●●●●○ | ●●●○○ | ●●●○○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●●○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **HighPerformance** | ●●○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●○○○ | ●●○○○ | ●●●●○ | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●●● | ●●●●● |
| **LowCost** | ●●●●○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●●●● | ●●●●● | ●●○○○ | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●○○○ | ●●●○○ | ●●●●○ | ●●○○○ |
| **Moderation** | ●●○○○ | ●●●○○ | ●●●○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●●○○○ | ●●●●○ | ●●●○○ | ●●○○○ | ●○○○○ | ●●●●○ | ●●●●○ | ●●●●● | ●●●●○ | ●●●○○ | ●●○○○ | ●●●●● | ●●●●● | ●●●●● | ●●●●● | ●●●●○ | ●●●○○ | ●●●●● | ●●●●● |
| **OpenWeight** | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ●●●●● | ●●●●● | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ | ○○○○○ |

How to interpret this mega‑heatmap
- It shows every family and every tier in one unified matrix.
- It visually encodes the hierarchy:
    - GPT‑5.2 > GPT‑5.1 > GPT‑5.0 > GPT‑4.x > GPT‑3.5
    - Pro > Base > Mini > Nano
    - Vision/Audio boosted on multimodal tags.
- It makes capability evolution across generations immediately visible.
- It is suitable for model‑selection engines.
